using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.Application.Features.Reportes.Commands;

public class UpdateReporteCommand : ICommand<Result>
{
    public int ReporteId { get; set; }
    public required ReporteEditDto Model { get; set; }
}

public class UpdateReporteCommandHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IValidator<ReporteEditDto> validator,
    IParameterExtractor parameterExtractor  
) : ICommandHandler<UpdateReporteCommand, Result>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<ReporteEditDto> _validator = validator;
    private readonly IParameterExtractor _parameterExtractor = parameterExtractor;

    public async Task<Result> HandleAsync(UpdateReporteCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = message.Model;
            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Invalid(validationResult.AsErrors());

            var oReporte = await _context.RSP_Reporte
                .SingleOrDefaultAsync(r => r.Id == message.ReporteId, cancellationToken);

            if (oReporte == null)
                return Result.NotFound($"No se encontró el reporte con Id {message.ReporteId}.");

            // Detectar si cambió el StoredProcedure
            var spChanged = !string.Equals(oReporte.StoredProcedure, model.StoredProcedure, StringComparison.OrdinalIgnoreCase);
            var oldStoredProcedure = oReporte.StoredProcedure;
            var newStoredProcedure = model.StoredProcedure;

            _mapper.Map(model, oReporte);
            _context.RSP_Reporte.Update(oReporte);
            await _context.SaveChangesAsync(cancellationToken);

            // ✅ Sincronizar parámetros según el caso
            if (spChanged && !string.IsNullOrWhiteSpace(newStoredProcedure))
            {
                // Caso 1: Cambió el SP - Reemplazar todos los parámetros
                await ReemplazarParametrosAsync(message.ReporteId, newStoredProcedure, cancellationToken);
            }
            else if (!spChanged && !string.IsNullOrWhiteSpace(newStoredProcedure))
            {
                // Caso 2: Mismo SP - Sincronizar cambios (parámetros nuevos/eliminados)
                await SincronizarParametrosAsync(message.ReporteId, newStoredProcedure, cancellationToken);
            }
            else if (string.IsNullOrWhiteSpace(newStoredProcedure))
            {
                // Caso 3: Se quitó el SP - Eliminar todos los parámetros
                await EliminarTodosParametrosAsync(message.ReporteId, cancellationToken);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    /// <summary>
    /// Reemplaza completamente los parámetros cuando cambia el SP
    /// </summary>
    private async Task ReemplazarParametrosAsync(int reporteId, string storedProcedure, CancellationToken cancellationToken)
    {
        // Eliminar parámetros existentes
        var parametrosExistentes = await _context.RSP_Parametro
            .Where(p => p.ReporteId == reporteId)
            .ToListAsync(cancellationToken);

        if (parametrosExistentes.Any())
            _context.RSP_Parametro.RemoveRange(parametrosExistentes);

        // Extraer y agregar nuevos parámetros
        var nuevosParametros = await ExtraerYMapearParametros(reporteId, storedProcedure, cancellationToken);

        if (nuevosParametros.Any())
        {
            await _context.RSP_Parametro.AddRangeAsync(nuevosParametros, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Sincroniza parámetros detectando adiciones, eliminaciones y modificaciones
    /// </summary>
    private async Task SincronizarParametrosAsync(int reporteId, string storedProcedure, CancellationToken cancellationToken)
    {
        // Obtener parámetros actuales del SP
        var parametrosSP = await _parameterExtractor.ExtractAsync(storedProcedure, cancellationToken);

        // Obtener parámetros guardados en BD
        var parametrosBD = await _context.RSP_Parametro
            .Where(p => p.ReporteId == reporteId)
            .ToListAsync(cancellationToken);

        // Crear diccionarios para fácil comparación
        var parametrosBDDict = parametrosBD.ToDictionary(p => p.NomParametro, p => p);
        var parametrosSPDict = parametrosSP.ToDictionary(p => p.Name, p => p);

        var cambiosDetectados = false;

        // 1. Detectar parámetros NUEVOS (en SP pero no en BD)
        var nuevosParametros = parametrosSP
            .Where(sp => !parametrosBDDict.ContainsKey(sp.Name))
            .ToList();

        if (nuevosParametros.Any())
        {
            cambiosDetectados = true;
            var parametrosParaAgregar = MapearParametros(reporteId, nuevosParametros);
            await _context.RSP_Parametro.AddRangeAsync(parametrosParaAgregar, cancellationToken);

            // Log o notificación
            Console.WriteLine($"📌 Nuevos parámetros detectados: {string.Join(", ", nuevosParametros.Select(p => p.Name))}");
        }

        // 2. Detectar parámetros ELIMINADOS (en BD pero no en SP)
        var parametrosEliminados = parametrosBD
            .Where(bd => !parametrosSPDict.ContainsKey(bd.NomParametro))
            .ToList();

        if (parametrosEliminados.Any())
        {
            cambiosDetectados = true;
            _context.RSP_Parametro.RemoveRange(parametrosEliminados);

            // Log o notificación
            Console.WriteLine($"🗑️ Parámetros eliminados: {string.Join(", ", parametrosEliminados.Select(p => p.NomParametro))}");
        }

        // 3. Detectar MODIFICACIONES (cambios en tipo de dato, orden, etc.)
        var parametrosModificados = new List<(RSP_Parametro Existente, ParameterDefinitionDto Nuevo)>();

        foreach (var bdParam in parametrosBD)
        {
            if (parametrosSPDict.TryGetValue(bdParam.NomParametro, out var spParam))
            {
                if (HaCambiado(bdParam, spParam))
                {
                    cambiosDetectados = true;
                    parametrosModificados.Add((bdParam, spParam));

                    // Log o notificación
                    Console.WriteLine($"✏️ Parámetro modificado: {bdParam.NomParametro} - Tipo: {bdParam.TipoDato} → {spParam.DataType}");
                }
            }
        }

        // Aplicar modificaciones
        foreach (var (existente, nuevo) in parametrosModificados)
        {
            existente.TipoDato = nuevo.DataType;
            existente.InputId = CalcularInputId(nuevo);
            existente.Order = nuevo.Order;
            // No actualizar campos manuales como TablaRef, ColumnaValor, etc.
        }

        // Guardar todos los cambios
        if (cambiosDetectados)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Elimina todos los parámetros de un reporte
    /// </summary>
    private async Task EliminarTodosParametrosAsync(int reporteId, CancellationToken cancellationToken)
    {
        var parametros = await _context.RSP_Parametro
            .Where(p => p.ReporteId == reporteId)
            .ToListAsync(cancellationToken);

        if (parametros.Any())
        {
            _context.RSP_Parametro.RemoveRange(parametros);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Extrae parámetros del SP y los mapea a entidades
    /// </summary>
    private async Task<List<RSP_Parametro>> ExtraerYMapearParametros(int reporteId, string storedProcedure, CancellationToken cancellationToken)
    {
        var parametrosSP = await _parameterExtractor.ExtractAsync(storedProcedure, cancellationToken);
        return MapearParametros(reporteId, parametrosSP);
    }

    /// <summary>
    /// Mapea una lista de definiciones de parámetros a entidades RSP_Parametro
    /// </summary>
    private List<RSP_Parametro> MapearParametros(int reporteId, IReadOnlyList<ParameterDefinitionDto> parametrosSP)
    {
        return parametrosSP.Select((param, index) => new RSP_Parametro
        {
            Id = Guid.NewGuid(),
            ReporteId = reporteId,
            NomParametro = param.Name,
            TipoDato = param.DataType,
            InputId = CalcularInputId(param),
            Display = param.Name.TrimStart('@'),
            Order = index,  // O usar param.Order si está disponible
            TablaRef = string.Empty,
            ColumnaValor = string.Empty,
            ColumnaTexto = string.Empty
        }).ToList();
    }

    /// <summary>
    /// Detecta si un parámetro ha cambiado
    /// </summary>
    private bool HaCambiado(RSP_Parametro existente, ParameterDefinitionDto nuevo)
    {
        return existente.TipoDato != nuevo.DataType ||
               existente.Order != nuevo.Order ||
               existente.InputId != CalcularInputId(nuevo);
    }

    /// <summary>
    /// Calcula el InputId basado en el tipo de dato
    /// </summary>
    private int CalcularInputId(ParameterDefinitionDto definition)
    {
        var tipoDato = definition.DataType.ToLower();

        return tipoDato switch
        {
            "int" or "bigint" or "varchar" or "nvarchar" or "decimal" or "tinyint" => 1,
            "bit" => 2,
            "datetime" or "date" or "datetime2" or "smalldatetime" => 3,
            _ => 0
        };
    }
}
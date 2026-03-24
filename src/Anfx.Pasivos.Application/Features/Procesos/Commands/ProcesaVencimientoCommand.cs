using Anfx.Pasivos.Application.Features.Procesos.DTOs;

namespace Anfx.Pasivos.Application.Features.Procesos.Commands;

public class ProcesaVencimientoCommand : ICommand<Result<ProcesaVencimientoResultDto>>
{


    public DateTime FechaInicial { get; set; }
    public DateTime FechaFinal { get; set; }
    public int? IdFondeador { get; set; }
    public int? IdContrato { get; set; }
}


internal class ProcesaVencimientoCommandHandler(IApplicationDbContext context) : ICommandHandler<ProcesaVencimientoCommand, Result<ProcesaVencimientoResultDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<ProcesaVencimientoResultDto>> HandleAsync(ProcesaVencimientoCommand message, CancellationToken cancellationToken = default)
    {
        try
        {

            // Validar que las fechas sean válidas
            if (message.FechaInicial > message.FechaFinal)
            {
                return Result.Invalid(new ValidationError("La fecha inicial no puede ser mayor a la fecha final"));
            }

            // Validar que el rango no sea demasiado amplio (opcional, según reglas de negocio)
            var diasRango = (message.FechaFinal - message.FechaInicial).Days;
            if (diasRango > 366) // Máximo un año
            {
                return Result.Invalid(new ValidationError("El rango de fechas no puede exceder los 366 días"));
            }

            // Validar que el contrato exista si se proporciona IdContrato
            if (message.IdContrato.HasValue)
            {
                var contrato = await _context.PSV_Contrato
                    .FirstOrDefaultAsync(c => c.IdContrato == message.IdContrato, cancellationToken);

                if (contrato == null)
                {
                    return Result.NotFound($"No se encontró el contrato con ID: {message.IdContrato}");
                }
            }

            // Validar que el fondeador exista si se proporciona IdFondeador
            if (message.IdFondeador.HasValue)
            {
                var fondeador = await _context.PSV_Fondeador
                    .FirstOrDefaultAsync(f => f.IdFondeador == message.IdFondeador, cancellationToken);

                if (fondeador == null)
                {
                    return Result.NotFound($"No se encontró el fondeador con ID: {message.IdFondeador}");
                }
            }

            var resultSp = await _context.Procedures.usp_PSV_ProcesaVencimientosAsync(
                message.IdFondeador,
                message.IdContrato,
                DateOnly.FromDateTime(message.FechaInicial),
                DateOnly.FromDateTime(message.FechaFinal)
            );
            var result = resultSp.FirstOrDefault();
            if (result == null) return Result.Error("Proceso no tubo resultados");
            if (!string.IsNullOrEmpty(result.Error))
            {
                return Result.Error(result.Error);

            }
            return Result.Success(new ProcesaVencimientoResultDto(result.TotalProcesados ?? 0), "Procesamiento Completo.");
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
using Anfx.Pasivos.Application.Features.Procesos.DTOs;

namespace Anfx.Pasivos.Application.Features.Procesos.Commands;

public class ProcesaMoratorioCommand : ICommand<Result<ProcesaMoratorioResultDto>>
{
    public DateTime? FechaProcesamiento { get; set; }
    public int? IdContrato { get; set; }
    public string ContratoPasivo { get; set; }

}

internal class ProcesaMoratorioCommandHandler(IApplicationDbContext context) : ICommandHandler<ProcesaMoratorioCommand, Result<ProcesaMoratorioResultDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<ProcesaMoratorioResultDto>> HandleAsync(ProcesaMoratorioCommand message, CancellationToken cancellationToken = default)
    {
        if (message.IdContrato == null && string.IsNullOrEmpty(message.ContratoPasivo))
        {
            return Result.Invalid(
                new ValidationError("Debe especificar un contrato o ejecutar el proceso global (sin filtro)"));
        }
        int? idContrato = message.IdContrato;

        if (idContrato == null && !string.IsNullOrEmpty(message.ContratoPasivo))
        {
            // Limpiar el formato si viene con descripción
            var contratoPasivo = message.ContratoPasivo.Split(new[] { " - " }, StringSplitOptions.None)[0];

            var contrato = await _context.PSV_Contrato
                .FirstOrDefaultAsync(c => c.Contrato == contratoPasivo, cancellationToken);

            if (contrato == null)
            {
                return Result.NotFound($"No se encontró el contrato: {message.ContratoPasivo}");
            }

            idContrato = contrato.IdContrato;
        }
        try
        {
            var resultSp = await _context.Procedures.usp_PSV_ProcesaMoratoriosAsync(
                message.FechaProcesamiento ?? DateTime.Now,
                message.IdContrato
            );
            var result = resultSp.FirstOrDefault();
            if (result == null) return Result.Error("Proceso no tubo resultados");
            if (!string.IsNullOrEmpty(result.Error))
            {
                return Result.Error(result.Error);

            }

            var mensaje = idContrato.HasValue
                ? $"Procesamiento completado para el contrato {message.ContratoPasivo ?? idContrato.ToString()}"
                : "Procesamiento global completado";

            return Result.Success(
                new ProcesaMoratorioResultDto(result.Procedimiento ?? ""),
                mensaje
            );
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
namespace Anfx.Pasivos.Application.Features.Contratos.Commands;

public class ActivarContratoCommand : ICommand<Result>
{

    public int IdContrato { get; set; }
    public DateTime FechaActivacion { get; set; }
}


internal class ActivarContratoCommandHandler(IApplicationDbContext context) : ICommandHandler<ActivarContratoCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> HandleAsync(ActivarContratoCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var resultSp = await _context.Procedures.usp_PSV_ActivarContratoAsync(
                        message.IdContrato,
                      DateOnly.FromDateTime(message.FechaActivacion), true);

            var result = resultSp.FirstOrDefault();

            if (result != null && !string.IsNullOrEmpty(result.Error))
            {
                return Result.Error(result.Error);
            }
            return Result.Success();

        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }

    }
}




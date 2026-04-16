namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class DeleteTasaValorCommand : ICommand<Result>
{
    public int IdTasaValor { get; set; }
}

internal class DeleteTasaValorCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteTasaValorCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> HandleAsync(DeleteTasaValorCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.TasaValor
                .SingleOrDefaultAsync(tv => tv.IdTasaValor == request.IdTasaValor, cancellationToken);

            if (entity == null)
            {
                return Result.NotFound("Valor de tasa no encontrado");
            }

            _context.TasaValor.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("El valor de tasa fue eliminado");
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}

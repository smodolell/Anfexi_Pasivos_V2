namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Commands;

public class DeleteTipoDireccionCommandHandler : ICommandHandler<DeleteTipoDireccionCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteTipoDireccionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Result> HandleAsync(DeleteTipoDireccionCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tipoDireccion = await _context.TiposDirecciones.SingleOrDefaultAsync(r => r.Id == request.Id);
            if (tipoDireccion == null)
            {
                return Result.NotFound("Tipo de dirección no encontrado");
            }
            _context.TiposDirecciones.Remove(tipoDireccion);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.SuccessWithMessage("Tipo de dirección eliminado exitosamente");
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);

        }
    }
}

namespace Anfx.Pasivos.Application.Features.Configuracion.Commands;

public class DeleteFondeadorCommand : ICommand<Result>
{
    public int Id { get; set; }
}


internal class DeleteFondeadorCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteFondeadorCommand, Result>
{
    private readonly IApplicationDbContext _context = context;
    public async Task<Result> HandleAsync(DeleteFondeadorCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var fondeador = await _context.PSV_Fondeador.SingleOrDefaultAsync(x => x.IdFondeador == request.Id);
            if (fondeador == null)
            {
                return Result.NotFound("Fondeador no encontrado");
            }
            _context.PSV_Fondeador.Remove(fondeador);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
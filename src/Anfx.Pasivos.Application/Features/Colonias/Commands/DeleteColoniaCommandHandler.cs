namespace Anfx.Pasivos.Application.Features.Colonias.Commands;

public class DeleteColoniaCommandHandler : ICommandHandler<DeleteColoniaCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteColoniaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Result> HandleAsync(DeleteColoniaCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var colonia = await _context.Colonias.FindAsync(request.Id);
            if (colonia == null)
            {
                return Result.NotFound($"Colonia con ID{request.Id} no encontrada");
            }


            _context.Colonias.Remove(colonia);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }

    }
}

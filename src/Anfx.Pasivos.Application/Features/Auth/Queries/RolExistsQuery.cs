namespace Anfx.Pasivos.Application.Features.Auth.Queries;

public record RolExistsQuery(int Id) : IQuery<Result<bool>>;

public class RolExistsQueryHandler : IQueryHandler<RolExistsQuery, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public RolExistsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Result<bool>> HandleAsync(RolExistsQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _context.Roles
                .AnyAsync(r => r.IdRol == request.Id /*&& r.Activo*/, cancellationToken);

            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al verificar la existencia del rol: {ex.Message}");
        }
    }
}

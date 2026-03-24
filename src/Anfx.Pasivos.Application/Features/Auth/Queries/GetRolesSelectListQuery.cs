namespace Anfx.Pasivos.Application.Features.Auth.Queries;

public record GetRolesSelectListQuery : IQuery<Result<List<SelectItemDto>>>;

public class GetRolesSelectListQueryHandler : IQueryHandler<GetRolesSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetRolesSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetRolesSelectListQuery message, CancellationToken cancellationToken = default)
    {
        try
        {
            var roles = await _context.Roles
                //.Where(r => r.Activo)
                .OrderBy(r => r.Titulo)
                .Select(r => new SelectItemDto
                {
                    Value = r.IdRol,
                    Text = r.Titulo ?? ""
                })
                .ToListAsync(cancellationToken);

            return Result.Success(roles);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener la lista de roles: {ex.Message}");
        }
    }
}

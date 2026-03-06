using Anfx.Pasivos.Application.Features.Usuarios.DTOs;

namespace Anfx.Pasivos.Application.Features.Usuarios.Queries;

public record GetUsuariosQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null) 
    : IQuery<Result<PagedResultDto<UsuarioDto>>>;

public class GetUsuariosQueryHandler : IQueryHandler<GetUsuariosQuery, Result<PagedResultDto<UsuarioDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<GetUsuariosQuery> _validator;

    public GetUsuariosQueryHandler(IApplicationDbContext context, IMapper mapper,IValidator<GetUsuariosQuery> validator)
    {
        _context = context;
        _mapper = mapper;
        this._validator = validator;
    }

    public async Task<Result<PagedResultDto<UsuarioDto>>> HandleAsync(GetUsuariosQuery request, CancellationToken cancellationToken = default)
    {
        try
        {

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }



            var query = _context.Usuarios
                .Include(u => u.Rol)
                .AsQueryable();

            // Aplicar filtro de búsqueda si se proporciona
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(u =>
                    u.NombreCompleto.Contains(request.SearchTerm) ||
                    u.Email.Contains(request.SearchTerm) ||
                    u.UsuarioNombre.Contains(request.SearchTerm) ||
                    u.Rol.sRol.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var usuarios = await query
                .OrderBy(u => u.NombreCompleto)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var usuarioDtos = _mapper.Map<List<UsuarioDto>>(usuarios);

            var pagedResult = new PagedResultDto<UsuarioDto>
            {
                Results = usuarioDtos,
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };

            return Result.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener los usuarios: {ex.Message}");
        }
    }
}

using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Features.Auth.Queries;

public record GetUsuariosQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null, bool? Activo = null,string? SortBy = null, string? SortDir = null)
    : IQuery<Result<PagedResultDto<UsuarioDto>>>;

public class GetUsuariosQueryHandler : IQueryHandler<GetUsuariosQuery, Result<PagedResultDto<UsuarioDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<GetUsuariosQuery> _validator;

    public GetUsuariosQueryHandler(IApplicationDbContext context, IMapper mapper, IValidator<GetUsuariosQuery> validator)
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

            // Filtro por estado activo
            if (request.Activo.HasValue)
                query = query.Where(u => u.Activo == request.Activo.Value);

            // Filtro de búsqueda
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(u =>
                    !(!u.NombreCompleto.Contains(request.SearchTerm) &&
                    !u.Email.Contains(request.SearchTerm) &&
                    !u.UserPass.Contains(request.SearchTerm) &&
                    (u.Rol.Titulo == null || !u.Rol.Titulo.Contains(request.SearchTerm))));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = (request.SortBy?.ToLower(), request.SortDir?.ToLower() == "desc") switch
            {
                ("id", true) => query.OrderByDescending(u => u.IdUsuario),
                ("id", false) => query.OrderBy(u => u.IdUsuario),
                ("nombrecompleto", true) => query.OrderByDescending(u => u.NombreCompleto),
                ("nombrecompleto", false) => query.OrderBy(u => u.NombreCompleto),
                ("email", true) => query.OrderByDescending(u => u.Email),
                ("email", false) => query.OrderBy(u => u.Email),
                ("usuarionombre", true) => query.OrderByDescending(u => u.UserName),
                ("usuarionombre", false) => query.OrderBy(u => u.UserName),
                _ => query.OrderBy(u => u.NombreCompleto),
            };


            var usuarios = await query
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

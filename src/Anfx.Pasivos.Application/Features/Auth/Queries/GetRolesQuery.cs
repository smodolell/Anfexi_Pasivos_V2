using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Features.Auth.Queries;

public class GetRolesQuery : IQuery<Result<PagedResultDto<RolDto>>>
{
    private static readonly HashSet<string> _validSortColumns = new()
    {
        nameof(RolDto.Id),
        nameof(RolDto.sRol),
        nameof(RolDto.Descripcion),
    };

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(RolDto.sRol);

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => value
        };
    }

    public string SortColumn
    {
        get => _sortColumn;
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(RolDto.sRol);
    }

    public bool SortDescending { get; set; } = true;
    public string? SearchTerm { get; set; }



}





public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, Result<PagedResultDto<RolDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDynamicSorter _sorter;
    private readonly IPaginator _paginator;

    public GetRolesQueryHandler(IApplicationDbContext context,
        IMapper mapper,
        IDynamicSorter sorter,
        IPaginator paginator)
    {
        _context = context;
        _mapper = mapper;
        _sorter = sorter;
        _paginator = paginator;
    }

    public async Task<Result<PagedResultDto<RolDto>>> HandleAsync(GetRolesQuery message, CancellationToken cancellationToken = default)
    {

        var query = _context.Roles.AsQueryable();//.Where(r => r.Activo);

        if (message.SearchTerm != null)
        {
            query = query.Where(r =>
            (r.Titulo != null && r.Titulo.Contains(message.SearchTerm)) ||
            (r.Descripcion != null && r.Descripcion.Contains(message.SearchTerm)));
        }


        query = _sorter.ApplySort(query, message.SortColumn, message.SortDescending);

        var result = await _paginator.PaginateAsync<Rol, RolDto>(
            query,
            message.Page,
            message.PageSize,
            cancellationToken
        );

        return Result.Success(result);
    }

    //public async Task<ApiResponseDto<PagedResultDto<RolDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        IQueryable<Rol> query = _context.Roles.Where(r => r.Activo);

    //        if (request.Sortby?.Any() == true)
    //        {
    //            var sortMap = new Dictionary<string, Expression<Func<Rol, object>>>(StringComparer.OrdinalIgnoreCase)
    //            {
    //                { "sRol", r => r.sRol },
    //                { "Descripcion", r => r.Descripcion }
    //            };

    //            bool isFirstSort = true;

    //            foreach (var sortingParameter in request.Sortby)
    //            {
    //                if (sortMap.TryGetValue(sortingParameter.Column ?? "", out var sortExpression))
    //                {
    //                    if (isFirstSort)
    //                    {
    //                        query = sortingParameter.Desc
    //                            ? query.OrderByDescending(sortExpression)
    //                            : query.OrderBy(sortExpression);

    //                        isFirstSort = false;
    //                    }
    //                    else
    //                    {
    //                        var orderedQuery = query as IOrderedQueryable<Rol>;
    //                        if (orderedQuery != null)
    //                        {
    //                            query = sortingParameter.Desc
    //                                ? orderedQuery.ThenByDescending(sortExpression)
    //                                : orderedQuery.ThenBy(sortExpression);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        else
    //            query = query.OrderBy(r => r.sRol);

    //        if (!string.IsNullOrEmpty(request.SearchTerm))
    //        {
    //            query = query.Where(r => r.sRol.Contains(request.SearchTerm) ||
    //                                   r.Descripcion.Contains(request.SearchTerm));
    //        }

    //        var totalCount = await query.CountAsync(cancellationToken);

    //        var roles = await query
    //            .Skip((request.Page - 1) * request.Size)
    //            .Take(request.Size)
    //            .ToListAsync(cancellationToken);

    //        var rolesDto = _mapper.Map<IEnumerable<RolDto>>(roles);

    //        var pagedResult = new PagedResultDto<RolDto>
    //        {
    //            Results = rolesDto,
    //            CurrentPage = request.Page,
    //            PageSize = request.Size,
    //            TotalCount = totalCount,
    //            TotalPages = (int)Math.Ceiling((double)totalCount / request.Size)
    //        };

    //        return ApiResponseDto<PagedResultDto<RolDto>>.SuccessResult(pagedResult);
    //    }
    //    catch (Exception ex)
    //    {
    //        return ApiResponseDto<PagedResultDto<RolDto>>.ErrorResult($"Error al obtener los roles: {ex.Message}");
    //    }
}

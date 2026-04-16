using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Queries;

public class GetTasasVariablesQuery : IQuery<Result<PagedResultDto<TasaVariableListItemDto>>>
{
    private static readonly HashSet<string> _validSortColumns = new()
    {
        nameof(TasaVariableListItemDto.Id),
        nameof(TasaVariableListItemDto.Nombre),
    };

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(TasaVariableListItemDto.Nombre);

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
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(TasaVariableListItemDto.Nombre);
    }

    public bool SortDescending { get; set; }

    public string? SearchText { get; set; }

    public bool? Activa { get; set; }
}

internal class GetTasasVariablesQueryHandler : IQueryHandler<GetTasasVariablesQuery, Result<PagedResultDto<TasaVariableListItemDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaginator _paginator;
    private readonly IDynamicSorter _sorter;

    public GetTasasVariablesQueryHandler(IApplicationDbContext context, IPaginator paginator, IDynamicSorter sorter)
    {
        _context = context;
        _paginator = paginator;
        _sorter = sorter;
    }

    public async Task<Result<PagedResultDto<TasaVariableListItemDto>>> HandleAsync(GetTasasVariablesQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Tasa
                .Include(t => t.TasaValors)
                .Where(t => t.EsVariable == true)
                .AsQueryable();
            if (request.Activa.HasValue)
            {
                query = query.Where(t => t.Activo == request.Activa.Value);
            }
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(t => t.Tasa1.Contains(request.SearchText));
            }

            var sortedQuery = _sorter.ApplySort(query, request.SortColumn, request.SortDescending);

            var result = await _paginator.PaginateAsync<Tasa, TasaVariableListItemDto>(
                sortedQuery,
                request.Page,
                request.PageSize
            );

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener las tasas variables: {ex.Message}");
        }
    }
}

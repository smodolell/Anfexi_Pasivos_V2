using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Queries;

public class GetEstatusContratosQuery : IQuery<Result<PagedResultDto<EstatusContratoListItemDto>>>
{
    private static readonly HashSet<string> _validSortColumns = new()
    {
        nameof(EstatusContratoListItemDto.Id),
        nameof(EstatusContratoListItemDto.EstatusContrato),
    };

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(EstatusContratoListItemDto.EstatusContrato);

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
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(EstatusContratoListItemDto.EstatusContrato);
    }

    public bool SortDescending { get; set; }

    public string? SearchText { get; set; }
}

internal class GetEstatusContratosQueryHandler : IQueryHandler<GetEstatusContratosQuery, Result<PagedResultDto<EstatusContratoListItemDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaginator _paginator;
    private readonly IDynamicSorter _sorter;

    public GetEstatusContratosQueryHandler(IApplicationDbContext context, IPaginator paginator, IDynamicSorter sorter)
    {
        _context = context;
        _paginator = paginator;
        _sorter = sorter;
    }

    public async Task<Result<PagedResultDto<EstatusContratoListItemDto>>> HandleAsync(GetEstatusContratosQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.PSV_EstatusContrato.AsQueryable();
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(ec => ec.EstatusContrato.Contains(request.SearchText));
            }
            var sortedQuery = _sorter.ApplySort(query, request.SortColumn, request.SortDescending);

            var result = await _paginator.PaginateAsync<PSV_EstatusContrato, EstatusContratoListItemDto>(
                sortedQuery,
                request.Page,
                request.PageSize
            );
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener los estatus de contrato: {ex.Message}");
        }
    }
}

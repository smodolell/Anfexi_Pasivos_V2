using Anfx.Pasivos.Application.Features.Reportes.DTOs;
using Anfx.Pasivos.Application.Features.Reportes.Specifications;

namespace Anfx.Pasivos.Application.Features.Reportes.Queries;

public class GetReportesQuery : IQuery<Result<PagedResultDto<ReporteListItemDto>>>
{
    private static readonly HashSet<string> _validSortColumns =
    [
        nameof(ReporteListItemDto.Id),
        nameof(ReporteListItemDto.NomReporte),
        nameof(ReporteListItemDto.StoredProcedure),
    ];

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(ReporteListItemDto.NomReporte);

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
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(ReporteListItemDto.NomReporte);
    }

    public bool SortDescending { get; set; }
    public string? SearchText { get; set; }
}

internal class GetReportesQueryHandler(
    IApplicationDbContext context,
    IPaginator paginator,
    IDynamicSorter sorter
) : IQueryHandler<GetReportesQuery, Result<PagedResultDto<ReporteListItemDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IPaginator _paginator = paginator;
    private readonly IDynamicSorter _sorter = sorter;

    public async Task<Result<PagedResultDto<ReporteListItemDto>>> HandleAsync(GetReportesQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var spec = new ReporteBySearchTextSpec(request.SearchText);
            var query = _context.RSP_Reporte.WithSpecification(spec);
            var sortedQuery = _sorter.ApplySort(query, request.SortColumn, request.SortDescending);

            var result = await _paginator.PaginateAsync<RSP_Reporte, ReporteListItemDto>(
                sortedQuery,
                request.Page,
                request.PageSize,
                cancellationToken
            );
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}

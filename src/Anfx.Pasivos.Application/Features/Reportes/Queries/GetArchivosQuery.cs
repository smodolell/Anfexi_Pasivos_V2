using Anfx.Pasivos.Application.Features.Reportes.DTOs;
using Anfx.Pasivos.Application.Features.Reportes.Specifications;

namespace Anfx.Pasivos.Application.Features.Reportes.Queries;

public class GetArchivosQuery : IQuery<Result<PagedResultDto<ArchivoListItemDto>>>
{
    private static readonly HashSet<string> _validSortColumns =
    [
        nameof(ArchivoListItemDto.Id),
        nameof(ArchivoListItemDto.NombreArchivo),
        nameof(ArchivoListItemDto.FechaCreacion),
        nameof(ArchivoListItemDto.ReporteId),
    ];

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(ArchivoListItemDto.FechaCreacion);

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
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(ArchivoListItemDto.FechaCreacion);
    }

    public bool SortDescending { get; set; } = true;
    public int? ReporteId { get; set; }
}

internal class GetArchivosQueryHandler(
    IApplicationDbContext context,
    IPaginator paginator,
    IDynamicSorter sorter
) : IQueryHandler<GetArchivosQuery, Result<PagedResultDto<ArchivoListItemDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IPaginator _paginator = paginator;
    private readonly IDynamicSorter _sorter = sorter;

    public async Task<Result<PagedResultDto<ArchivoListItemDto>>> HandleAsync(GetArchivosQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var spec = new ArchivoByReporteIdSpec(request.ReporteId);
            var query = _context.RSP_Archivo
                .Include(a => a.RSP_Reporte)
                .WithSpecification(spec);
            var sortedQuery = _sorter.ApplySort(query, request.SortColumn, request.SortDescending);

            var result = await _paginator.PaginateAsync<RSP_Archivo, ArchivoListItemDto>(
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

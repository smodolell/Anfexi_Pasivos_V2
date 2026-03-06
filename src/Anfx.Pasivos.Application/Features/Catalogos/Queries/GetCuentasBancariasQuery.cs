using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Queries;

public class GetCuentasBancariasQuery : IQuery<Result<PagedResultDto<CuentaBancariaListItemDto>>>
{
    private static readonly HashSet<string> _validSortColumns = new()
    {
        nameof(CuentaBancariaListItemDto.Id),
        nameof(CuentaBancariaListItemDto.CuentaBancaria),
        nameof(CuentaBancariaListItemDto.Banco),
        nameof(CuentaBancariaListItemDto.CLABE),
    };

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(CuentaBancariaListItemDto.CuentaBancaria);

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
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(CuentaBancariaListItemDto.CuentaBancaria);
    }

    public bool SortDescending { get; set; }

    public string? SearchText { get; set; }
}

internal class GetCuentasBancariasQueryHandler : IQueryHandler<GetCuentasBancariasQuery, Result<PagedResultDto<CuentaBancariaListItemDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaginator _paginator;
    private readonly IDynamicSorter _sorter;

    public GetCuentasBancariasQueryHandler(IApplicationDbContext context, IPaginator paginator, IDynamicSorter sorter)
    {
        _context = context;
        _paginator = paginator;
        _sorter = sorter;
    }

    public async Task<Result<PagedResultDto<CuentaBancariaListItemDto>>> HandleAsync(GetCuentasBancariasQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.PSV_CuentaBancaria.Include(cb => cb.PSV_Banco).AsQueryable();
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(cb => cb.CuentaBancaria.Contains(request.SearchText) || cb.CLABE.Contains(request.SearchText) || cb.PSV_Banco.Banco.Contains(request.SearchText));
            }
            var sortedQuery = _sorter.ApplySort(query, request.SortColumn, request.SortDescending);

            var result = await _paginator.PaginateAsync<PSV_CuentaBancaria, CuentaBancariaListItemDto>(
                sortedQuery,
                request.Page,
                request.PageSize
            );
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener las cuentas bancarias: {ex.Message}");
        }
    }
}

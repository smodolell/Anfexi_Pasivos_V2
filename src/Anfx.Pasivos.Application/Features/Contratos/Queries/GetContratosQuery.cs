using Anfx.Pasivos.Application.Features.Contratos.DTOs;
using Anfx.Pasivos.Application.Features.Contratos.Specifications;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetContratosQuery : IQuery<Result<PagedResultDto<ContratoPasivoListItem>>>
{

    private static readonly HashSet<string> _validSortColumns = new()
    {
        nameof(ContratoPasivoListItem.ID),
        nameof(ContratoPasivoListItem.Contrato),
    };

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(ContratoPasivoListItem.Contrato);

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
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(ContratoPasivoListItem.Contrato);
    }

    public bool SortDescending { get; set; }
    public int? IdFondeador { get; set; }
    public int? IdEstatusContrato { get; set; }
    public int? IdLineaCredito { get; set; }
    public string? SearchText { get; set; }


}


internal class GetContratosQueryHandler(IApplicationDbContext context, IPaginator paginator, IDynamicSorter sorter) : IQueryHandler<GetContratosQuery, Result<PagedResultDto<ContratoPasivoListItem>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IPaginator _paginator = paginator;
    private readonly IDynamicSorter _sorter = sorter;

    public async Task<Result<PagedResultDto<ContratoPasivoListItem>>> HandleAsync(GetContratosQuery message, CancellationToken cancellationToken = default)
    {
        var spec = new View_ContratoPasivoSpec(
            message.IdFondeador,
            message.IdEstatusContrato,
            message.IdLineaCredito,
            message.SearchText
        );
        var query = _context.View_ContratoPasivo.WithSpecification(spec);

        var sortedQuery = _sorter.ApplySort(query, message.SortColumn, message.SortDescending);

        var result = await _paginator.PaginateAsync<View_ContratoPasivo, ContratoPasivoListItem>(
            sortedQuery,
            message.Page,
            message.PageSize
        );
        return Result.Success(result);




    }
}
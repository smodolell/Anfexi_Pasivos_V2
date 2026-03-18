using Anfx.Pasivos.Application.Features.Contratos.DTOs;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetRelActivoPasivoQuery : IQuery<Result<PagedResultDto<RelActivoPasivoDto>>>
{
    private static readonly HashSet<string> _validSortColumns = new()
    {
        nameof(RelActivoPasivoDto.Contrato),
        nameof(RelActivoPasivoDto.Capital),
        nameof(RelActivoPasivoDto.TipoCredito),
        nameof(RelActivoPasivoDto.Fondeador),
    };

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(RelActivoPasivoDto.Contrato);

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
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(RelActivoPasivoDto.Contrato);
    }

    public bool SortDescending { get; set; }

    public string? SearchText { get; set; }
    public int IdFondeador { get; set; }
    public int IdContrato { get; set; }

}
internal class GetRelActivoPasivoQueryHandler(IApplicationDbContext context,IPaginator paginator,IDynamicSorter sorter
    ) : IQueryHandler<GetRelActivoPasivoQuery, Result<PagedResultDto<RelActivoPasivoDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IPaginator _paginator = paginator;
    private readonly IDynamicSorter _sorter = sorter;

    public async Task<Result<PagedResultDto<RelActivoPasivoDto>>> HandleAsync(GetRelActivoPasivoQuery message, CancellationToken cancellationToken = default)
    {
        var query = _context.View_RelActivoPasivo
            .Where(w => w.IdFondeador == message.IdFondeador
                && (w.IdContratoPasivo == message.IdContrato || w.IdContratoPasivo == null)
                && w.IdContratoPasivoLC == message.IdContrato);

        if (!string.IsNullOrWhiteSpace(message.SearchText))
        {
            query = query.Where(w =>
                w.Contrato.Contains(message.SearchText) ||
                w.TipoCredito.Contains(message.SearchText) ||
                w.Fondeador.Contains(message.SearchText));
        }

        query = _sorter.ApplySort(query, message.SortColumn, message.SortDescending);

        var result = await _paginator.PaginateAsync<View_RelActivoPasivo, RelActivoPasivoDto>(
            query,
            message.Page,
            message.PageSize,
            cancellationToken);

        return Result.Success(result);
    }
}

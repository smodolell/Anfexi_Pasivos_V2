using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Queries;

public class GetLineasCreditoQuery : IQuery<Result<PagedResultDto<LineaCreditoListItemDto>>>
{
    private static readonly HashSet<string> _validSortColumns = new()
    {
        nameof(LineaCreditoListItemDto.ID),
        //nameof(LineaCreditoListItemDto.Fondeador),
        //nameof(LineaCreditoListItemDto.Moneda),
        nameof(LineaCreditoListItemDto.MontoAprobado),
        nameof(LineaCreditoListItemDto.FechaAprobacion),
        //nameof(LineaCreditoListItemDto.Activo)
    };

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(LineaCreditoListItemDto.FechaAprobacion);

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
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(LineaCreditoListItemDto.FechaAprobacion);
    }

    public bool SortDescending { get; set; }
    public string? SearchText { get; set; }
    public int? IdFondeador { get; set; }
    public int? IdMoneda { get; set; }
    public bool? Activo { get; set; }
}

public class GetLineasCreditoQueryHandler(
    IApplicationDbContext context,
    IPaginator paginator,
    IDynamicSorter sorter
) : IQueryHandler<GetLineasCreditoQuery, Result<PagedResultDto<LineaCreditoListItemDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IPaginator _paginator = paginator;
    private readonly IDynamicSorter _sorter = sorter;

    public async Task<Result<PagedResultDto<LineaCreditoListItemDto>>> HandleAsync(
        GetLineasCreditoQuery request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.View_LineaCredito.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                //query = query.Where(lc =>
                    //lc..Contains(request.SearchText) ||
                    //lc.Moneda.Contains(request.SearchText));
            }

            if (request.IdFondeador.HasValue)
            {
                query = query.Where(lc => lc.FondeadorID == request.IdFondeador.Value);
            }

            //if (request.IdMoneda.HasValue)
            //{
            //    query = query.Where(lc => lc.IdMoneda == request.IdMoneda.Value);
            //}

            //if (request.Activo.HasValue)
            //{
            //    query = query.Where(lc => lc.Activo == request.Activo.Value);
            //}

            var sortedQuery = _sorter.ApplySort(query, request.SortColumn, request.SortDescending);

            var result = await _paginator.PaginateAsync<View_LineaCredito, LineaCreditoListItemDto>(
                sortedQuery,
                request.Page,
                request.PageSize,
                cancellationToken
            );

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener las líneas de crédito: {ex.Message}");
        }
    }
}


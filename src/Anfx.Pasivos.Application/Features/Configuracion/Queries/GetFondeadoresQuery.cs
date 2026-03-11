
using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Queries;

public class GetFondeadoresQuery : IQuery<Result<PagedResultDto<FondeadorListItemDto>>>
{
    private static readonly HashSet<string> _validSortColumns = new()
    {
        nameof(FondeadorListItemDto.ID),
        nameof(FondeadorListItemDto.Titulo),
        nameof(FondeadorListItemDto.Contratos),
        nameof(FondeadorListItemDto.ClaveCuentaContable),
    };

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(FondeadorListItemDto.Titulo);

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
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(FondeadorListItemDto.Titulo);
    }

    public bool SortDescending { get; set; }

    public string? SearchText { get; set; }

}


public class GetFondeadoresQueryHandler(
    IApplicationDbContext context, 
    IPaginator paginator, 
    IDynamicSorter sorter
) : IQueryHandler<GetFondeadoresQuery, Result<PagedResultDto<FondeadorListItemDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IPaginator _paginator = paginator;
    private readonly IDynamicSorter _sorter = sorter;

    public async Task<Result<PagedResultDto<FondeadorListItemDto>>> HandleAsync(GetFondeadoresQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.View_Fondeador.AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(tc => tc.Titulo.Contains(request.SearchText));
            }
            var sortedQuery = _sorter.ApplySort(query, request.SortColumn, request.SortDescending);

            var result = await _paginator.PaginateAsync<View_Fondeador, FondeadorListItemDto>(
                sortedQuery,
                request.Page,
                request.PageSize
            );
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener los fondeadores: {ex.Message}");
        }
    }
}
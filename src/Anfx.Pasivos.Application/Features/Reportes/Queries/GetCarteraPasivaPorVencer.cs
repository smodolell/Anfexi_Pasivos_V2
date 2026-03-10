using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.Application.Features.Reportes.Queries;

public class GetCarteraPasivaPorVencer : IQuery<Result<PagedResultDto<CarteraReporteDto>>>
{
    public string? SearchText { get; set; }

    public int? IdFondeador { get; set; }
    public int? IdContratoPasivo { get; set; }
    public int? IdContratoActivo { get; set; }
    public int Saldos { get; set; }

    private static readonly HashSet<string> _validSortColumns = new()
    {
        nameof(CarteraReporteDto.Contrato),
        nameof(CarteraReporteDto.FecVencimiento),
        nameof(CarteraReporteDto.Capital),
        nameof(CarteraReporteDto.Interes),
        nameof(CarteraReporteDto.Total),
    };

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(CarteraReporteDto.FecVencimiento);

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
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(CarteraReporteDto.FecVencimiento);
    }

    public bool SortDescending { get; set; }
}



internal class GetCarteraPasivaPorVencerHandler : IQueryHandler<GetCarteraPasivaPorVencer, Result<PagedResultDto<CarteraReporteDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaginator _paginator;
    private readonly IDynamicSorter _sorter;

    public GetCarteraPasivaPorVencerHandler(IApplicationDbContext context, IPaginator paginator, IDynamicSorter sorter)
    {
        _context = context;
        _paginator = paginator;
        _sorter = sorter;
    }

    public async Task<Result<PagedResultDto<CarteraReporteDto>>> HandleAsync(
        GetCarteraPasivaPorVencer request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.View_CarteraPasiva_PorVencer.AsQueryable(); // Cambio aquí a la vista de pasiva

            if (!String.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(w => w.ContratoPasivo.Contains(request.SearchText));
            }

            if (request.IdFondeador != null)
            {
                query = query.Where(w => w.IdFondeador == request.IdFondeador);
            }

            if (request.IdContratoPasivo != null)
            {
                query = query.Where(w => w.IdContratoPasivo == request.IdContratoPasivo);
            }

            if (request.IdContratoActivo != null)
            {
                query = query.Where(w => w.IdContratoActivo == request.IdContratoActivo);
            }

            var sortedQuery = _sorter.ApplySort(query, request.SortColumn, request.SortDescending);

            var result = await _paginator.PaginateAsync<View_CarteraPasiva_PorVencer, CarteraReporteDto>( // Cambio aquí
                sortedQuery,
                request.Page,
                request.PageSize,
                cancellationToken
            );

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener cartera pasiva por vencer: {ex.Message}");
        }
    }
}
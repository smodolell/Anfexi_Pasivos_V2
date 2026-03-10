using Anfx.Pasivos.ApiService.Responces.Reportes;
using Anfx.Pasivos.Application.Features.Reportes.DTOs;
using Anfx.Pasivos.Application.Features.Reportes.Queries;
using Anfx.Pasivos.Application.Features.SelectLists.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Reportes : EndpointGroupBase
{
    public override string? GroupName => "reportes";
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            .WithTags("Reportes");

        group.MapGet("dashboard", GetDashboard)
            .WithName("GetDashboard")
            .WithSummary("Obtiene datos del dashboard de reportes")
            .WithDescription("Obtiene información de cartera activa, pasiva y otros datos para el dashboard")
            .Produces<ApiResponseDto<DashboardResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("cartera-por-vencer/{Page?}/{PageSize?}", GetCarteraPorVencer)
            .WithName("GetCarteraPorVencer")
            .WithSummary("Obtiene cartera activa por vencer con paginación")
            .WithDescription("Obtiene listado paginado de cartera activa próxima a vencer")
            .Produces<ApiResponseDto<PagedResultDto<CarteraReporteDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("cartera-pasiva-por-vencer", GetCarteraPasivaPorVencer)
            .WithName("GetCarteraPasivaPorVencer")
            .WithSummary("Obtiene cartera pasiva por vencer con paginación")
            .Produces<ApiResponseDto<PagedResultDto<CarteraReporteDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);



    }

    public async Task<IResult> GetDashboard(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] int? IdFondeador = null,
        [FromQuery] int? IdContratoPasivo = null,
        [FromQuery] int? IdContratoActivo = null,
        [FromQuery] int? Saldos = null,
        CancellationToken cancellationToken = default
    )
    {


        var dataActivo = await queryMediator.QueryAsync(new GetCarteraActivaQuery
        {
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1

        });

        var dataPasivos = await queryMediator.QueryAsync(new GetCarteraPasivaQuery
        {
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1

        });

        var dataMensualActiva = await queryMediator.QueryAsync(new GetCarteraActivaMensualQuery
        {
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1

        });

        var dataMensualPasiva = await queryMediator.QueryAsync(new GetCarteraPasivaMensualQuery
        {
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1

        });


        var result = new DashboardResponse
        {
            Activos = dataActivo.Value,
            Pasivos = dataPasivos.Value,
            ActivosMensual = dataMensualActiva.Value,
            PasivosMensual = dataMensualPasiva.Value
        };

        return Result.Success(result)
            .ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetCarteraPorVencer(
        [FromServices] IQueryMediator queryMediator,
        [FromRoute] int? Page = 1,
        [FromRoute] int? PageSize = 10,
        [FromQuery] string? SearchText = null,
        [FromQuery] int? IdFondeador = null,
        [FromQuery] int? IdContratoPasivo = null,
        [FromQuery] int? IdContratoActivo = null,
        [FromQuery] int? Saldos = null,
        [FromQuery] string SortColumn = "FecVencimiento",
        [FromQuery] bool SortDescending = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetCarteraActivaPorVencer
        {
            SearchText = SearchText,
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1,
            Page = Page ?? 1,
            PageSize = PageSize ?? 10,
            SortColumn = SortColumn,
            SortDescending = SortDescending
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetCarteraPasivaPorVencer(
       [FromServices] IQueryMediator queryMediator,
       [FromQuery] int Page = 1,
       [FromQuery] int PageSize = 10,
       [FromQuery] string? SearchText = null,
       [FromQuery] int? IdFondeador = null,
       [FromQuery] int? IdContratoPasivo = null,
       [FromQuery] int? IdContratoActivo = null,
       [FromQuery] int? Saldos = null,
       [FromQuery] string SortColumn = "FecVencimiento",
       [FromQuery] bool SortDescending = false,
       CancellationToken cancellationToken = default)
    {
        var query = new GetCarteraPasivaPorVencer
        {
            SearchText = SearchText,
            IdFondeador = IdFondeador,
            IdContratoPasivo = IdContratoPasivo,
            IdContratoActivo = IdContratoActivo,
            Saldos = Saldos ?? 1,
            Page = Page,
            PageSize = PageSize,
            SortColumn = SortColumn,
            SortDescending = SortDescending
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }


  
}

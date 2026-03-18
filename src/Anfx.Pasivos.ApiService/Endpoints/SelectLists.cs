using Anfx.Pasivos.Application.Features.SelectLists.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class SelectLists : EndpointGroupBase
{
    public override string? GroupName => "select-lists";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            .WithTags("Select Lists");

        group.MapGet("fondeadores", GetFondeadoresSelectList)
            .WithName("GetFondeadoresSelectList")
            .WithSummary("Obtiene fondeadores")
            .WithDescription("Retorna una lista de los Fondeadores")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError); ;

        // Endpoint específico para contratos pasivos por fondeador
        group.MapGet("contratos-pasivos-por-fondeador/{idFondeador}", GetContratosPasivosPorFondeador)
            .WithName("GetContratosPasivosPorFondeador")
            .WithSummary("Obtiene contratos pasivos por ID de fondeador")
            .WithDescription("Retorna una lista de contratos pasivos filtrados por el ID del fondeador")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("contratos-activos-por-pasivo/{idContratoPasivo}", GetContratosActivosPorPasivo)
            .WithName("GetContratosActivosPorPasivo")
            .WithSummary("Obtiene contratos activos por ID de contrato pasivo")
            .WithDescription("Retorna una lista de contratos activos filtrados por el ID del contrato pasivo")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("moneda/", GetMonedas)
            .WithName("GetMonedas")
            .WithSummary("Obtiene las monedas del catalogo")
            .WithDescription("Retorna una lista de monedas")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("tipo-movimiento/", GetTipoMovimientos)
            .WithName("GetTipoMovimientos")
            .WithSummary("Obtiene los tipo de Movimientos del catalogo")
            .WithDescription("Retorna una lista de tipo de Movimiento")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
    }



    public async Task<IResult> GetFondeadoresSelectList(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? maxResults = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFondeadoresSelectListQuery
        {
            SearchTerm = searchTerm,
            MaxResults = maxResults
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);

        return Result.Success(result.Value).ToCustomMinimalApiResult();
    }



    public async Task<IResult> GetContratosPasivosPorFondeador(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int idFondeador,
    [FromQuery] bool? estatusActivo,
    CancellationToken cancellationToken = default)
    {
        var query = new GetContratoPasivoByIdFondeadorSelectListQuery
        {
            IdFondeador = idFondeador,
            EstatusActivo = estatusActivo,
        };
        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetContratosActivosPorPasivo(
        [FromServices] IQueryMediator queryMediator,
        [FromRoute] int idContratoPasivo,
        CancellationToken cancellationToken = default)
    {

        var query = new GetContratoActivoByPasivoSelectListQuery
        {
            IdContratoPasivo = idContratoPasivo
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);

        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetMonedas(
      [FromServices] IQueryMediator queryMediator,
      CancellationToken cancellationToken = default)
    {

        var query = new GetMonedasSelectListQuery();
        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetTipoMovimientos(
     [FromServices] IQueryMediator queryMediator,
     CancellationToken cancellationToken = default)
    {

        var query = new GetTipoMovimientoSelectListQuery();
        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }
}
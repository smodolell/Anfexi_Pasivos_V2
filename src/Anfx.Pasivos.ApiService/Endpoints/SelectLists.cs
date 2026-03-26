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

        
        group.MapGet("tasas/", GetTasas)
            .WithName("GetTasas")
            .WithSummary("Obtiene lista de tasas para select")
            .WithDescription("Retorna un listado de tasas filtradas por el tipo (fija o variable) para ser utilizadas en controles de selección.")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("tipo-terminaciones/", GetTipoTerminaciones)
            .WithName("GetTipoTerminaciones")
            .WithSummary("Obtiene lista de tipos de terminación para select")
            .WithDescription("Retorna un listado de todos los tipos de terminación disponibles para ser utilizados en controles de selección.")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("tipo-reduccion/", GetTipoReduccion)
            .WithName("GetTipoReduccion")
            .WithSummary("Obtiene lista de tipos de reducción para select")
            .WithDescription("Retorna un listado de los tipos de reducción disponibles (ej. POR MONTO) para ser utilizados en controles de selección.")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("tipo-pagos/", GetTipoPagos)
            .WithName("GetTipoPagos")
            .WithSummary("Obtiene lista de tipos de pago para select")
            .WithDescription("Retorna un listado de todos los tipos de pago disponibles para ser utilizados en controles de selección.")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("bancos/", GetBancosSelectList)
            .WithName("GetBancosSelectList")
            .WithSummary("Obtiene lista de bancos para select")
            .WithDescription("Retorna un listado de todos los bancos disponibles para ser utilizados en controles de selección.")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("cuentas-bancarias/{idBanco}", GetCuentaBancariaByBancoIdSelectList)
            .WithName("GetCuentaBancariaByBancoIdSelectList")
            .WithSummary("Obtiene lista de cuentas bancarias por banco")
            .WithDescription("Retorna un listado de cuentas bancarias filtradas por el ID del banco para ser utilizadas en controles de selección dependientes.")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("lineas-credito/{idFondeador}", GetLineasCreditoByFondeador)
            .WithName("GetLineasCreditoByFondeador")
            .WithSummary("Obtiene líneas de crédito por ID de fondeador")
            .WithDescription("Retorna una lista de líneas de crédito filtradas por el ID del fondeador")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("estatus-contrato/", GetEstatusContratoSelectList)
            .WithName("GetEstatusContratoSelectList")
            .WithSummary("Obtiene lista de estatus de contrato")
            .WithDescription("Retorna un listado de todos los estatus de contrato disponibles para ser utilizados en controles de selección")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("tipos-credito", GetTipoCreditoSelectList)
            .WithName("GetTipoCreditoSelectList")
            .WithSummary("Obtiene lista de tipos de crédito")
            .WithDescription("Retorna un listado de todos los tipos de crédito disponibles para ser utilizados en controles de selección")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // Agregar este endpoint dentro del método Map
        group.MapGet("periodicidades", GetPeriodicidadSelectList)
            .WithName("GetPeriodicidadSelectList")
            .WithSummary("Obtiene lista de periodicidades")
            .WithDescription("Retorna un listado de todas las periodicidades disponibles para ser utilizadas en controles de selección")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // Agregar este endpoint dentro del método Map
        group.MapGet("tasas-iva", GetTasaIvaSelectList)
            .WithName("GetTasaIvaSelectList")
            .WithSummary("Obtiene lista de tasas de IVA")
            .WithDescription("Retorna un listado de las tasas de IVA disponibles (16%, 11%, 0%) para ser utilizadas en controles de selección")
            .Produces<ApiResponseDto<List<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("periodicidades-tta/{idTipoTablaAmortiza}", GetPeriodicidadTTASelectList)
            .WithName("GetPeriodicidadTTASelectList")
            .WithSummary("Obtiene lista de periodicidades por tipo de tabla de amortización")
            .WithDescription("Retorna un listado de periodicidades filtradas por el ID del tipo de tabla de amortización para ser utilizadas en controles de selección dependientes")
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

    public async Task<IResult> GetTasas(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] bool? esVariable,
        CancellationToken cancellationToken = default)
    {

        var query = new GetTasaSelectListQuery
        {
            EsVariable = esVariable ?? false
        };
        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetTipoTerminaciones(
    [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetTipoTerminacionesSelectListQuery();

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetTipoReduccion(
        [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetTipoReduccionSelectListQuery();

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetTipoPagos(
    [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetTipoPagoSelectListQuery();

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }
    public async Task<IResult> GetBancosSelectList(
    [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetBancoSelectListQuery();

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetCuentaBancariaByBancoIdSelectList(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int idBanco)
    {
        var query = new GetCuentaBancariaByBancoIdSelectListQuery
        {
            IdBanco = idBanco
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetLineasCreditoByFondeador(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int idFondeador,
    CancellationToken cancellationToken = default)
    {
        var query = new GetLineaCreditoByFondeadorSelectListQuery
        {
            IdFondeador = idFondeador
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetEstatusContratoSelectList(
    [FromServices] IQueryMediator queryMediator,
    CancellationToken cancellationToken = default)
    {
        var query = new GetEstatusContratoSelectListQuery();
        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetTipoCreditoSelectList(
    [FromServices] IQueryMediator queryMediator,
    CancellationToken cancellationToken = default)
    {
        var query = new GetTipoCreditoSelectListQuery();
        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPeriodicidadSelectList(
        [FromServices] IQueryMediator queryMediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPeriodicidadSelectListQuery();
        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetTasaIvaSelectList(
    [FromServices] IQueryMediator queryMediator,
    CancellationToken cancellationToken = default)
    {
        var query = new GetTasaIvaSelectListQuery();
        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPeriodicidadTTASelectList(
        [FromServices] IQueryMediator queryMediator,
        [FromRoute] int idTipoTablaAmortiza,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPeriodicidadTTASelectListQuery
        {
            IdTipoTablaAmortiza = idTipoTablaAmortiza
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }
}
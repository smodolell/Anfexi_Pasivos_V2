using Anfx.Pasivos.ApiService.Responces.Contratos;
using Anfx.Pasivos.Application.Features.Contratos.Commands;
using Anfx.Pasivos.Application.Features.Contratos.DTOs;
using Anfx.Pasivos.Application.Features.Contratos.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;


public class Contratos : EndpointGroupBase
{

    public override string? GroupName => "contratos";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            .WithTags("Contratos");

        // Endpoint para obtener información general de un contrato pasivo
        group.MapGet("{contratoPasivo}/info-general", GetInfoGeneral)
            .WithName("GetInfoGeneralContratoPasivo")
            .WithSummary("Obtiene información general de un contrato pasivo")
            .WithDescription("Retorna información general, saldo vencido, movimientos capturables y saldo insoluto de un contrato pasivo")
            .Produces<ApiResponseDto<InfoGeneralContratoPasivoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("tabla-amortiza/{idContrato}", GetTablaAmortizacionByTipo)
         .WithName("GetTablaAmortizacionByTipo")
         .WithSummary("Obtiene la tabla de amortización de un contrato por tipo")
         .Produces<ApiResponseDto<List<TablaAmortizaItemDto>>>(StatusCodes.Status200OK)
         .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
         .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("rel-activo-pasivo", GetRelActivoPasivo)
            .WithName("GetRelActivoPasivo")
              .WithSummary("Obtiene relación activo-pasivo por contrato y fondeador")
    .WithDescription("Retorna información paginada de la relación activo-pasivo, incluyendo contrato, capital, tipo de crédito y fondeador, filtrada por ID de fondeador y ID de contrato con soporte para búsqueda y ordenamiento dinámico")
            .Produces<ApiResponseDto<PagedResultDto<RelActivoPasivoDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPut("contrato-pasivo/{id}/asignar-activos", AsignarPasivos)
            .WithName("AsignarPasivos")
            .WithSummary("Asigna contratos activos a un contrato pasivo")
            .WithDescription("Permite asignar o reasignar contratos activos a un contrato pasivo específico. Si se envía ContratosActivos, se asigna un solo contrato; si se envía ListaContratos, se asignan múltiples contratos.")
            .Accepts<AsignarPasivosResponce>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("anticipo/{contrato}", GetAnticipoByContrato)
            .WithName("GetAnticipoByContrato")
            .WithSummary("Obtiene información para anticipo por contrato pasivo")
            .WithDescription("Retorna la configuración inicial para realizar un anticipo sobre un contrato pasivo activo específico, incluyendo la fecha sugerida basada en la tabla de amortización.")
            .Produces<ApiResponseDto<AnticipoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("anticipo/config", GetAnticipoConfig)
            .WithName("GetAnticipoConfig")
            .WithSummary("Obtiene configuración para anticipo/liquidación de contrato")
            .WithDescription("Retorna la configuración necesaria para procesar un anticipo o liquidación de contrato basado en el tipo de terminación seleccionado, incluyendo montos, porcentajes de IVA y validaciones específicas.")
            .Produces<ApiResponseDto<AnticipoConfigDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("anticipo/interes", GetInteres)
            .WithName("GetInteres")
            .WithSummary("Calcula el interés para un anticipo")
            .WithDescription("Calcula el monto de intereses generados por un anticipo en un contrato pasivo, basado en la fecha de corte y el monto anticipado.")
            .Produces<ApiResponseDto<decimal>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("movimiento/{idMovimiento}/detalle", GetMovimientoDetalle)
            .WithName("GetMovimientoDetalle")
            .WithSummary("Obtiene el detalle de un movimiento")
            .WithDescription("Retorna la información detallada de un movimiento específico, incluyendo los pagos aplicados")
            .Produces<ApiResponseDto<MovimientoDetalleDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);



        group.MapGet("pago/{idPago}/detalle", GetPagoDetalle)
            .WithName("GetPagoDetalle")
            .WithSummary("Obtiene el detalle de un pago")
            .WithDescription("Retorna la información detallada de un pago específico, incluyendo los movimientos o conceptos asociados.")
            .Produces<ApiResponseDto<PagoDetalleDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("caja/{contrato}", GetCajaByContrato)
            .WithName("GetCajaByContrato")
            .WithSummary("Obtiene información de caja por contrato pasivo")
            .WithDescription("Retorna la configuración inicial para registrar un pago en caja para un contrato específico.")
            .Produces<ApiResponseDto<CajaDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);



        group.MapPost("caja/confirmar", ConfirmarPagoCaja)
            .WithName("ConfirmarPagoCaja")
            .WithSummary("Confirma y procesa un pago de caja")
            .WithDescription("Registra un pago en caja, aplicando los movimientos seleccionados mediante el procedimiento almacenado usp_PSV_PagarCargosAsync.")
            .Accepts<CajaDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


     
    }

    public async Task<IResult> GetInfoGeneral(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] string contratoPasivo,
    CancellationToken cancellationToken = default)
    {

        var query = new GetInfoGeneralQuery
        {
            ContratoPasivo = contratoPasivo
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetTablaAmortizacionByTipo(
        [FromServices] IQueryMediator queryMediator,
        [FromRoute] int idContrato,
        [FromQuery] int? idTipoTabla,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetGetTablaAmortizaQuery
        {
            IdContrato = idContrato,
            IdTipoTabla = idTipoTabla ?? 1
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);


        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetRelActivoPasivo(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] int idFondeador,
        [FromQuery] int idContrato,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string sortColumn = nameof(RelActivoPasivoDto.Contrato),
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default
    )
    {

        var query = new GetRelActivoPasivoQuery
        {
            IdFondeador = idFondeador,
            IdContrato = idContrato,
            SearchText = q,
            PageSize = size,
            Page = page,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> AsignarPasivos(
    [FromServices] ICommandMediator commandMediator,
    [FromRoute] int id,
    [FromBody] AsignarPasivosResponce model)
    {
        var command = new AsignarPasivosCommand
        {
            IdContratoPasivo = id,
            ListaContratos = model.ListaContratos,
            ContratosActivos = model.ContratosActivos
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetAnticipoByContrato(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] string contrato)
    {
        var query = new GetAnticipoQuery
        {
            ContratoPasivo = contrato
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetAnticipoConfig(
    [FromServices] IQueryMediator queryMediator,
    [FromQuery] int idTipoTerminacion,
    [FromQuery] int idContrato)
    {
        var query = new GetAnticipoConfigQuery
        {
            IdTipoTerminacion = idTipoTerminacion,
            IdContrato = idContrato
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetInteres(
    [FromServices] IQueryMediator queryMediator,
    [FromQuery] int idContrato,
    [FromQuery] DateOnly fechaCorte,
    [FromQuery] decimal montoAnticipo)
    {
        var query = new GetInteresQuery
        {
            IdContrato = idContrato,
            FechaCorte = fechaCorte,
            MontoAnticipo = montoAnticipo
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> ConfirmarAnticipo(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] AnticipoDto request)
    {
        var command = new AnticipoConfirmCommand
        {
            IdContrato = request.IdContrato,
            IdTipoTerminacion = request.IdTipoTerminacion,
            IdTipoReduccion = request.IdTipoReduccion,
            FechaAnticipo = request.FechaAnticipo,
            MontoAnticipo = request.MontoAnticipo,
            MontoInteres = request.MontoInteres,
            MontoPena = request.MontoPena,
            MontoIVA_Interes = request.MontoIVA_Interes,
            MontoIVA_Pena = request.MontoIVA_Pena,
            MontoTotal = request.MontoTotal,
            EsLiquidacion = request.EsLiquidacion
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetMovimientoDetalle(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int idMovimiento)
    {
        var query = new GetMovimientoDetalleQuery
        {
            IdMovimiento = idMovimiento
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetPagoDetalle(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int idPago)
    {
        var query = new GetPagosDetalleQuery
        {
            IdPago = idPago
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetCajaByContrato(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] string contrato)
    {
        var query = new GetCajaQuery
        {
            ContratoPasivo = contrato
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> ConfirmarPagoCaja(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] CajaDto request)
    {
        var command = new CajaConfirmCommand
        {
            Model = request
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }



}

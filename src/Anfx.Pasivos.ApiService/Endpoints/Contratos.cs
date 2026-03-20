using Anfx.Pasivos.ApiService.Responces.Contratos;
using Anfx.Pasivos.Application.Features.Contratos.Commands;
using Anfx.Pasivos.Application.Features.Contratos.DTOs;
using Anfx.Pasivos.Application.Features.Contratos.Queries;
using Anfx.Pasivos.Domain.Entities;
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


        group.MapGet("autocomplete", GetAutocompleteContrato)
            .WithName("GetAutocompleteContrato")
            .WithSummary("Autocompleta contratos pasivos")
            .WithDescription("Retorna una lista de contratos pasivos que coinciden con el término de búsqueda, limitado a 15 resultados, para ser utilizado en controles de autocompletado.")
            .Produces<ApiResponseDto<List<AutocompleteResultDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
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


        if (result.IsSuccess)
        {
            var queryTabla = new GetGetTablaAmortizaQuery
            {
                IdContrato = result.Value.IdContrato,
                IdTipoTabla = 1
            };
            result.Value.TablaAmortiza = await queryMediator.QueryAsync(queryTabla);
        }

        

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

    public async Task<IResult> GetAutocompleteContrato(
    [FromServices] IQueryMediator queryMediator,
    [FromQuery] string? search)
    {
        var query = new GetAutocompleteContratoQuery
        {
            Search = search
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }



}

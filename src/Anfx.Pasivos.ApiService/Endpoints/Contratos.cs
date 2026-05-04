using Anfx.Pasivos.ApiService.Requests.Contratos;
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



        group.MapGet("/{id}", GetContratoById)
            .WithName("GetContratoById")
            .WithSummary("Obtiene un contrato pasivo por su ID")
            .WithDescription("Retorna la información detallada de un contrato pasivo específico")
            .Produces<ApiResponseDto<ContratoPasivoEditDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("/", GetContratosPasivos)
            .WithName("GetContratosPasivos")
            .WithSummary("Lista contratos pasivos con filtros")
            .WithDescription("Retorna una lista paginada de contratos pasivos con soporte para filtros por fondeador, estatus, línea de crédito, búsqueda por texto, ordenamiento dinámico y paginación")
            .Produces<ApiResponseDto<PagedResultDto<ContratoPasivoListItem>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("nuevo/{idLineaCredito}", GetAddContrato)
            .WithName("GetAddContrato")
            .WithSummary("Obtiene datos iniciales para crear un nuevo contrato")
            .WithDescription("Retorna información pre-cargada basada en la línea de crédito seleccionada para facilitar la creación de un nuevo contrato")
            .Produces<ApiResponseDto<ContratoPasivoEditDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized);

        group.MapPost("/", CreateContrato)
           .WithName("CreateContrato")
           .WithSummary("Crea un nuevo contrato pasivo")
           .WithDescription("Crea un nuevo contrato pasivo con toda su información incluyendo pagos irregulares si aplica")
           .Accepts<ContratoPasivoEditDto>("application/json")
           .Produces<ApiResponseDto<int>>(StatusCodes.Status200OK)
           .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
           .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
           .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", UpdateContrato)
            .WithName("UpdateContrato")
            .WithSummary("Actualiza un contrato pasivo existente")
            .WithDescription("Actualiza la información de un contrato pasivo existente. Solo permite modificar contratos en estado 'Borrador'")
            .Accepts<ContratoPasivoEditDto>("application/json")
            .Produces<ApiResponseDto>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapPut("/{id}/activar", ActivarContrato)
            .WithName("ActivarContrato")
            .WithSummary("Activa un contrato pasivo")
            .WithDescription("Activa un contrato pasivo con la fecha de activación especificada")
            .Accepts<ActivarContratoRequest>("application/json")
            .Produces<ApiResponseDto>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("clave-contrato/{idTipoCredito}", GetClaveContrato)
            .WithName("GetClaveContrato")
            .WithSummary("Obtiene la clave del contrato")
            .WithDescription("Genera y retorna la clave del contrato basada en el prefijo, sufijo y contador del tipo de crédito")
            .Produces<ApiResponseDto<string>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // Agregar este endpoint dentro del método Map
        group.MapGet("tipo-tabla-amortiza/{id}/info", GetTipoTablaAmortizaInfo)
            .WithName("GetTipoTablaAmortizaInfo")
            .WithSummary("Obtiene información de capitalización por tipo de tabla de amortización")
            .WithDescription("Retorna si es capitalizable y las listas de tipos de capitalización y pago de capital según el tipo de tabla de amortización")
            .Produces<ApiResponseDto<TipoTablaAmortizaInfoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("{idContrato}/pagos", GetPagosByIdContrato)
            .WithName("GetPagosByIdContrato")
            .WithSummary("Obtiene los pagos de un contrato")
            .WithDescription("Retorna la lista de pagos asociados a un contrato específico")
            .Produces<ApiResponseDto<List<PagoItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("{idContrato}/movimientos", GetMovimientosByIdContrato)
            .WithName("GetMovimientosByIdContrato")
            .WithSummary("Obtiene los movimientos de un contrato")
            .WithDescription("Retorna la lista de movimientos asociados a un contrato específico")
            .Produces<ApiResponseDto<List<MovimientoItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("{idContratoPasivo}/contratos-asignados", GetContratosAsignados)
            .WithName("GetContratosAsignados")
            .WithSummary("Obtiene los contratos asignados a un contrato pasivo")
            .WithDescription("Retorna la lista de contratos activos asignados a un contrato pasivo específico")
            .Produces<ApiResponseDto<List<ContratosAsignadosDto>>>(StatusCodes.Status200OK)
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

    public async Task<IResult> GetContratoById(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int id,
    CancellationToken cancellationToken = default)
    {
        var query = new GetContratoByIdQuery
        {
            IdContrato = id
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetContratosPasivos(
      [FromServices] IQueryMediator queryMediator,
      [FromQuery] int? idFondeador = null,
      [FromQuery] int? idEstatusContrato = null,
      [FromQuery] int? idLineaCredito = null,
      [FromQuery] string? searchText = null,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string sortColumn = "Contrato",
      [FromQuery] bool sortDescending = false,
      CancellationToken cancellationToken = default)
    {
        var query = new GetContratosQuery
        {
            IdFondeador = idFondeador,
            IdEstatusContrato = idEstatusContrato,
            IdLineaCredito = idLineaCredito,
            SearchText = searchText,
            Page = page,
            PageSize = pageSize,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetAddContrato(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int idLineaCredito,
    CancellationToken cancellationToken = default)
    {
        var query = new GetAddContratoQuery
        {
            IdLineaCredito = idLineaCredito
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateContrato(
      [FromServices] ICommandMediator commandMediator,
      [FromBody] ContratoPasivoEditDto model,
      CancellationToken cancellationToken = default)
    {
        var command = new SaveContratoCommand
        {
            IdContrato = 0,
            Model = model
        };

        var result = await commandMediator.SendAsync(command, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> UpdateContrato(
      [FromServices] ICommandMediator commandMediator,
      [FromRoute] int id,
      [FromBody] ContratoPasivoEditDto model,
      CancellationToken cancellationToken = default)
    {
        var command = new SaveContratoCommand
        {
            IdContrato = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> ActivarContrato(
    [FromServices] ICommandMediator commandMediator,
    [FromRoute] int id,
    [FromBody] ActivarContratoRequest request,
    CancellationToken cancellationToken = default)
    {
        var command = new ActivarContratoCommand
        {
            IdContrato = id,
            FechaActivacion = request.FechaActivacion
        };

        var result = await commandMediator.SendAsync(command, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetClaveContrato(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int idTipoCredito,
    CancellationToken cancellationToken = default)
    {
        var query = new GetClaveContratoQuery
        {
            IdTipoCredito = idTipoCredito
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetTipoTablaAmortizaInfo(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int id,
    CancellationToken cancellationToken = default)
    {
        var query = new GetTipoTablaAmortizaInfoQuery
        {
            IdTipoTablaAmortiza = id
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPagosByIdContrato(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int idContrato,
    CancellationToken cancellationToken = default)
    {
        var query = new GetPagosByIdContratoQuery
        {
            IdContrato = idContrato
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetMovimientosByIdContrato(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int idContrato,
    CancellationToken cancellationToken = default)
    {
        var query = new GetMovimientosByIdContratoQuery
        {
            IdContrato = idContrato
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetContratosAsignados(
    [FromServices] IQueryMediator queryMediator,
    [FromRoute] int idContratoPasivo,
    CancellationToken cancellationToken = default)
    {
        var query = new GetContratosAsignadosQuery
        {
            IdContratoPasivo = idContratoPasivo
        };

        var result = await queryMediator.QueryAsync(query, cancellationToken);
        return result.ToCustomMinimalApiResult();
    }
}

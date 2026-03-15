using Anfx.Pasivos.Application.Features.Catalogos.Commands;
using Anfx.Pasivos.Application.Features.Catalogos.DTOs;
using Anfx.Pasivos.Application.Features.Catalogos.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Catalogos : EndpointGroupBase
{
    public override string? GroupName => "catalogos";
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            .RequireAuthorization()
        .WithTags("Catalogos");

        #region Banco
        // GET by id
        group.MapGet("banco/{id}", GetBancoById)
            .WithName("GetBancoById")
            .WithSummary("Obtiene un banco por ID")
            .Produces<ApiResponseDto<BancoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("banco/", GetPaginatedBanco)
            .WithSummary("Obtiene bancos paginados y filtrados")
            .Produces<ApiResponseDto<PagedResultDto<BancoListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("banco/", CreateBanco)
            .WithName("CreateBanco")
            .WithSummary("Crea un nuevo banco")
            .Accepts<BancoDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);

        group.MapPut("banco/{id}", UpdateBanco)
            .WithName("UpdateBanco")
            .WithSummary("Actualiza un banco")
            .Accepts<BancoDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
        #endregion

        #region CuentaBancaria
        // GET by id
        group.MapGet("cuentaBancaria/{id}", GetCuentasBancariaById)
            .WithName("GetCuentaBancariaById")
            .WithSummary("Obtiene una cuenta bancaria por ID")
            .Produces<ApiResponseDto<CuentaBancariaDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("cuentaBancaria", GetPaginatedCuentasBancaria)
            .WithSummary("Obtiene cuentas bancarias paginadas y filtradas")
            .Produces<ApiResponseDto<PagedResultDto<CuentaBancariaListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("cuentaBancaria", CreateCuentaBancaria)
            .WithName("CreateCuentaBancaria")
            .WithSummary("Crea una nueva cuenta bancaria")
            .Accepts<CuentaBancariaDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);

        group.MapPut("cuentaBancaria/{id}", UpdateCuentaBancaria)
            .WithName("UpdateCuentaBancaria")
            .WithSummary("Actualiza una cuenta bancaria")
            .Accepts<CuentaBancariaDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
        #endregion

        #region Estatus Contrato

        // GET by id
        group.MapGet("estatus-contrato/{id}", GetEstatusContratoById)
            .WithName("GetEstatusContratoById")
            .WithSummary("Obtiene un estatus de contrato por ID")
            .Produces<ApiResponseDto<EstatusContratoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("estatus-contrato/", GetPaginatedEstatusContratos)
            .WithSummary("Obtiene estatus de contrato paginados y filtrados")
            .Produces<ApiResponseDto<PagedResultDto<EstatusContratoListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("estatus-contrato/", CreateEstatusContrato)
            .WithName("CreateEstatusContrato")
            .WithSummary("Crea un nuevo estatus de contrato")
            .Accepts<EstatusContratoDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);

        group.MapPut("estatus-contrato/{id}", UpdateEstatusContrato)
            .WithName("UpdateEstatusContrato")
            .WithSummary("Actualiza un estatus de contrato")
            .Accepts<EstatusContratoDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
        #endregion

        #region TipoPago


        // GET by id
        group.MapGet("tipo-pago/{id}", GetTipoPagoById)
            .WithName("GetTipoPagoById")
            .WithSummary("Obtiene un tipo de pago por ID")
            .Produces<ApiResponseDto<TipoPagoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("tipo-pago/", GetPaginatedTipoPago)
            .WithSummary("Obtiene tipos de pago paginados y filtrados")
            .Produces<ApiResponseDto<PagedResultDto<TipoPagoListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("tipo-pago/", CreateTipoPago)
            .WithName("CreateTipoPago")
            .WithSummary("Crea un nuevo tipo de pago")
            .Accepts<TipoPagoDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);

        group.MapPut("tipo-pago/{id}", UpdateTipoPago)
            .WithName("UpdateTipoPago")
            .WithSummary("Actualiza un tipo de pago")
            .Accepts<TipoPagoDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
        #endregion

    }

    #region Banco

    public async Task<IResult> GetBancoById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var result = await queryMediator.QueryAsync(new GetBancoByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginatedBanco(
    IQueryMediator queryMediator,
    [FromQuery] string? q = null,
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    [FromQuery] string sortColumn = nameof(BancoListItemDto.Id),
    [FromQuery] bool sortDescending = false)
    {
        var query = new GetBancosQuery
        {
            SearchText = q,
            PageSize = size,
            Page = page,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateBanco(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] BancoDto model)
    {
        var command = new CreateBancoCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateBanco(
    [FromServices] ICommandMediator commandMediator,
  [FromRoute] int id,
    [FromBody] BancoDto model)
    {
        var command = new UpdateBancoCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    #endregion

    #region CuentaBancaria

    public async Task<IResult> GetCuentasBancariaById(
     [FromServices] IQueryMediator queryMediator,
    [FromRoute] int id)
    {
        var result = await queryMediator.QueryAsync(new GetCuentaBancariaByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginatedCuentasBancaria(
    IQueryMediator queryMediator,
    [FromQuery] string? q = null,
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    [FromQuery] string sortColumn = nameof(CuentaBancariaListItemDto.Id),
    [FromQuery] bool sortDescending = false)
    {
        var query = new GetCuentasBancariasQuery
        {
            SearchText = q,
            PageSize = size,
            Page = page,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateCuentaBancaria(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] CuentaBancariaDto model)
    {
        var command = new CreateCuentaBancariaCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateCuentaBancaria(
    [FromServices] ICommandMediator commandMediator,
    int id,
    [FromBody] CuentaBancariaDto model)
    {
        var command = new UpdateCuentaBancariaCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    #endregion

    #region Estatus Contrato


    public async Task<IResult> GetEstatusContratoById(
     [FromServices] IQueryMediator queryMediator,
    [FromRoute] int id)
    {
        var result = await queryMediator.QueryAsync(new GetEstatusContratoByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginatedEstatusContratos(
    IQueryMediator queryMediator,
    [FromQuery] string? q = null,
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    [FromQuery] string sortColumn = nameof(EstatusContratoListItemDto.Id),
    [FromQuery] bool sortDescending = false)
    {
        var query = new GetEstatusContratosQuery
        {
            SearchText = q,
            PageSize = size,
            Page = page,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateEstatusContrato(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] EstatusContratoDto model)
    {
        var command = new CreateEstatusContratoCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateEstatusContrato(
    [FromServices] ICommandMediator commandMediator,
   [FromRoute] int id,
    [FromBody] EstatusContratoDto model)
    {
        var command = new UpdateEstatusContratoCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    #endregion

    #region TipoPago

    public async Task<IResult> GetTipoPagoById(
 [FromServices] IQueryMediator queryMediator,
 int id)
    {
        var result = await queryMediator.QueryAsync(new GetTipoPagoByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginatedTipoPago(
    IQueryMediator queryMediator,
    [FromQuery] string? q = null,
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    [FromQuery] string sortColumn = nameof(TipoPagoListItemDto.Id),
    [FromQuery] bool sortDescending = false)
    {
        var query = new GetTipoPagosQuery
        {
            SearchText = q,
            PageSize = size,
            Page = page,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateTipoPago(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] TipoPagoDto model)
    {
        var command = new CreateTipoPagoCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateTipoPago(
    [FromServices] ICommandMediator commandMediator,
  [FromRoute] int id,
    [FromBody] TipoPagoDto model)
    {
        var command = new UpdateTipoPagoCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    #endregion
}

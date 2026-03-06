using Anfx.Pasivos.Application.Features.Configuracion.Commands;
using Anfx.Pasivos.Application.Features.Configuracion.Dtos;
using Anfx.Pasivos.Application.Features.Configuracion.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Configuraciones : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {

        var group = groupBuilder.MapGroup("/")
           .WithTags("Configuraciones");


        // GET by id
        group.MapGet("tipo-credito/{id}", GetTipoCreditoById)
            .WithName("GetTipoCreditoById")
            .WithSummary("Tipo de Credito")
            .WithDescription("Obtiene un tipo de Crédito por ID")
            .Produces<ApiResponseDto<TipoCreditoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("tipo-credito/", GetPaginatedTipoCredito)
            .WithName("GetPaginatedTipoCreditoById")
            .WithSummary("Obtiene tipo de Crédito paginadas y filtradas")
            .Produces<ApiResponseDto<PagedResultDto<TipoCreditoListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("tipo-credito/", CreateTipoCredito)
            .WithName("CreateTipoCredito")
            .WithSummary("Crea un nuevo tipo de credito pasivo")
            .Accepts<TipoCreditoDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);


        group.MapPut("tipo-credito/{id}", UpdateTipoCredito)
            .WithName("UpdateTipoCredito")
            .WithSummary("Actualiza un tipo de credito")
            .Accepts<TipoCreditoDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        // GET by id
        group.MapGet("tipo-tabla-amortiza/{id}", GetTipoTablaAmortizaById)
            .WithName("GetTipoTablaAmortizaById")
            .WithSummary("Obtiene un tipo de tabla amortiza por ID")
            .Produces<ApiResponseDto<TipoTablaAmortizaDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("tipo-tabla-amortiza/", GetPaginatedTipoTablaAmortiza)
            .WithSummary("Obtiene tipos de tabla amortiza paginados y filtrados")
            .Produces<ApiResponseDto<PagedResultDto<TipoTablaAmortizaListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("tipo-tabla-amortiza/", CreateTipoTablaAmortiza)
            .WithName("CreateTipoTablaAmortiza")
            .WithSummary("Crea un nuevo tipo de tabla amortiza")
            .Accepts<TipoTablaAmortizaDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);

        group.MapPut("tipo-tabla-amortiza/{id}", UpdateTipoTablaAmortiza)
            .WithName("UpdateTipoTablaAmortiza")
            .WithSummary("Actualiza un tipo de tabla amortiza")
            .Accepts<TipoTablaAmortizaDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
    }


    public async Task<IResult> GetTipoCreditoById(
     [FromServices] IQueryMediator queryMediator,
     int id)
    {
        var result = await queryMediator.QueryAsync(new GetTipoCreditoByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();

    }

    public async Task<IResult> GetPaginatedTipoCredito(
    IQueryMediator queryMediator,
    [FromQuery] string? q = null,
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    [FromQuery] string sortColumn = nameof(TipoCreditoListItemDto.Id),
    [FromQuery] bool sortDescending = false)
    {
        var query = new GetTipoCreditosQuery
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

    public async Task<IResult> CreateTipoCredito(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] TipoCreditoDto model)
    {
        var command = new CreateTipoCreditoCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> UpdateTipoCredito(
    [FromServices] ICommandMediator commandMediator,
    int id,
    [FromBody] TipoCreditoDto model)
    {
        var command = new UpdateTipoCreditoCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    #region TipoTablaAmortiza


    public async Task<IResult> GetTipoTablaAmortizaById(
     [FromServices] IQueryMediator queryMediator,
     int id)
    {
        var result = await queryMediator.QueryAsync(new GetTipoTablaAmortizaByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginatedTipoTablaAmortiza(
    IQueryMediator queryMediator,
    [FromQuery] string? q = null,
    [FromQuery] int page = 1,
    [FromQuery] int size = 10,
    [FromQuery] string sortColumn = nameof(TipoTablaAmortizaListItemDto.Id),
    [FromQuery] bool sortDescending = false)
    {
        var query = new GetTipoTablaAmortizasQuery
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

    public async Task<IResult> CreateTipoTablaAmortiza(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] TipoTablaAmortizaDto model)
    {
        var command = new CreateTipoTablaAmortizaCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateTipoTablaAmortiza(
    [FromServices] ICommandMediator commandMediator,
    int id,
    [FromBody] TipoTablaAmortizaDto model)
    {
        var command = new UpdateTipoTablaAmortizaCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    #endregion
}

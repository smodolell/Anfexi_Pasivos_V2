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



        group.MapGet("fondeador/", GetPaginatedFondeadores)
            .WithName("GetPaginatedFondeadores")
            .WithSummary("Obtiene fondeadores paginados y filtrados")
            .WithDescription("Obtiene una lista paginada de fondeadores con opciones de filtrado y ordenamiento")
            .Produces<ApiResponseDto<PagedResultDto<FondeadorListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("fondeador/{id}", GetFondeadorById)
            .WithName("GetFondeadorById")
            .WithSummary("Obtiene un fondeador por ID")
            .WithDescription("Obtiene un fondeador específico por su identificador")
            .Produces<ApiResponseDto<FondeadorDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // POST create
        group.MapPost("fondeador/", CreateFondeador)
            .WithName("CreateFondeador")
            .WithSummary("Crea un nuevo fondeador")
            .WithDescription("Crea un nuevo registro de fondeador")
            .Accepts<FondeadorEditDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);

        // PUT update
        group.MapPut("fondeador/{id}", UpdateFondeador)
            .WithName("UpdateFondeador")
            .WithSummary("Actualiza un fondeador existente")
            .WithDescription("Actualiza los datos de un fondeador específico")
            .Accepts<FondeadorEditDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // DELETE
        group.MapDelete("fondeador/{id}", DeleteFondeador)
            .WithName("DeleteFondeador")
            .WithSummary("Elimina un fondeador")
            .WithDescription("Elimina un fondeador específico por su ID")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapGet("linea-credito/{id}", GetLineaCreditoById)
            .WithName("GetLineaCreditoById")
            .WithSummary("Obtiene una línea de crédito por ID")
            .WithDescription("Obtiene una línea de crédito específica por su identificador")
            .Produces<ApiResponseDto<LineaCreditoDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // GET paginated list
        group.MapGet("linea-credito/", GetPaginatedLineasCredito)
            .WithName("GetPaginatedLineasCredito")
            .WithSummary("Obtiene líneas de crédito paginadas y filtradas")
            .WithDescription("Obtiene una lista paginada de líneas de crédito con opciones de filtrado y ordenamiento")
            .Produces<ApiResponseDto<PagedResultDto<LineaCreditoListItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // POST create
        group.MapPost("linea-credito/", CreateLineaCredito)
            .WithName("CreateLineaCredito")
            .WithSummary("Crea una nueva línea de crédito")
            .WithDescription("Crea un nuevo registro de línea de crédito")
            .Accepts<LineaCreditoEditDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto<int>>(StatusCodes.Status500InternalServerError);

        // PUT update
        group.MapPut("linea-credito/{id}", UpdateLineaCredito)
            .WithName("UpdateLineaCredito")
            .WithSummary("Actualiza una línea de crédito existente")
            .WithDescription("Actualiza los datos de una línea de crédito específica")
            .Accepts<LineaCreditoEditDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // DELETE
        group.MapDelete("linea-credito/{id}", DeleteLineaCredito)
            .WithName("DeleteLineaCredito")
            .WithSummary("Elimina una línea de crédito")
            .WithDescription("Elimina una línea de crédito específica por su ID")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("linea-credito/{idLineaCredito}/tipos-credito", GetTiposCreditoByLineaCredito)
            .WithName("GetTiposCreditoByLineaCredito").WithSummary("Obtiene tipos de crédito por línea de crédito")
            .WithDescription("Obtiene todos los tipos de crédito activos indicando cuáles están seleccionados para la línea de crédito especificada")
            .Produces<ApiResponseDto<List<RelLineaCreditoTipoCreditoDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // PUT/POST: Guarda la relación entre línea de crédito y tipos de crédito
        group.MapPut("linea-credito/{idLineaCredito}/tipos-credito", SaveTiposCreditoByLineaCredito)
            .WithName("SaveTiposCreditoByLineaCredito")
            .WithSummary("Guarda los tipos de crédito asociados a una línea")
            .WithDescription("Actualiza la selección de tipos de crédito para una línea de crédito específica")
            .Accepts<List<RelLineaCreditoTipoCreditoDto>>("application/json")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);
    }



    #region TipoCredito

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
    #endregion



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


    #region Fondeador


    public async Task<IResult> GetPaginatedFondeadores(
     [FromServices] IQueryMediator queryMediator,
     [FromQuery] string? q = null,
     [FromQuery] int page = 1,
     [FromQuery] int size = 10,
     [FromQuery] string sortColumn = nameof(FondeadorListItemDto.Titulo),
     [FromQuery] bool sortDescending = false)
    {
        var query = new GetFondeadoresQuery
        {
            SearchText = q,
            Page = page,
            PageSize = size,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetFondeadorById(
      [FromServices] IQueryMediator queryMediator,
      int id)
    {
        var result = await queryMediator.QueryAsync(new GetFondeadorByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateFondeador(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] FondeadorEditDto model)
    {
        var command = new CreateFondeadorCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateFondeador(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] FondeadorEditDto model)
    {
        var command = new UpdateFondeadorCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> DeleteFondeador(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var command = new DeleteFondeadorCommand
        {
            Id = id
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    #endregion


    #region LineaCredito

    private async Task<IResult> GetLineaCreditoById(
    [FromServices] IQueryMediator queryMediator,
    int id)
    {
        var result = await queryMediator.QueryAsync(new GetLineaCreditoByIdQuery { Id = id });
        return result.ToCustomMinimalApiResult();
    }

    private async Task<IResult> GetPaginatedLineasCredito(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int? idFondeador = null,
        [FromQuery] int? idMoneda = null,
        [FromQuery] bool? activo = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string sortColumn = nameof(LineaCreditoListItemDto.FechaAprobacion),
        [FromQuery] bool sortDescending = true)
    {
        var query = new GetLineasCreditoQuery
        {
            SearchText = q,
            IdFondeador = idFondeador,
            IdMoneda = idMoneda,
            Activo = activo,
            Page = page,
            PageSize = size,
            SortColumn = sortColumn,
            SortDescending = sortDescending
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    private async Task<IResult> CreateLineaCredito(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] LineaCreditoEditDto model)
    {
        var command = new CreateLineaCreditoCommand
        {
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    private async Task<IResult> UpdateLineaCredito(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] LineaCreditoEditDto model)
    {
        var command = new UpdateLineaCreditoCommand
        {
            Id = id,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    private async Task<IResult> DeleteLineaCredito(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var command = new DeleteLineaCreditoCommand
        {
            Id = id
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }
    #endregion

    #region MyRegion


    private async Task<IResult> GetTiposCreditoByLineaCredito(
        [FromServices] IQueryMediator queryMediator,
        int idLineaCredito)
    {
        var query = new GetTiposCreditoByIdLineaCreditoQuery
        {
            IdLineaCredito = idLineaCredito
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    private async Task<IResult> SaveTiposCreditoByLineaCredito(
        [FromServices] ICommandMediator commandMediator,
        int idLineaCredito,
        [FromBody] List<RelLineaCreditoTipoCreditoDto> model)
    {
        var command = new SaveTiposCreditoByIdLineaCreditoCommand
        {
            IdLineaCredito = idLineaCredito,
            Model = model
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    #endregion

}

using Anfx.Pasivos.Application.Features.Sistema.Commands;
using Anfx.Pasivos.Application.Features.Sistema.DTOs;
using Anfx.Pasivos.Application.Features.Sistema.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;


namespace Anfx.Pasivos.ApiService.Endpoints;

public class Sistema : EndpointGroupBase
{
    public override string? GroupName => "sistema";
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            //.RequireAuthorization()
            .WithTags("Sistema");


        #region Empresa
        group.MapPost("empresa/", CreateEmpresa)
            .WithName("CreateEmpresa")
            .WithSummary("Crea una nueva empresa")
            .Accepts<EmpresaCreateDto>("application/json")
            .Produces<EmpresaCreateDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);


        group.MapPut("empresa/{id}", UpdateEmpresa)
            .WithName("UpdateEmpresa")
            .WithSummary("Actualiza una empresa existente")
            .Accepts<EmpresaUpdateDto>("application/json")
            .Produces<EmpresaDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapDelete("empresa/{id}", DeleteEmpresa)
            .WithName("DeleteEmpresa")
            .WithSummary("Elimina una empresa")
            .Produces<EmpresaDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("empresa/", GetEmpresaPaginated)
            .WithSummary("Obtiene empresas paginadas y filtradas")
            .Produces<PagedResultDto<EmpresaDto>>(StatusCodes.Status200OK);


        group.MapGet("empresa/all", GetEmpresaAll)
            .WithName("get-all")
            .WithSummary("Obtiene todas las empresas activas")
            .Produces<List<EmpresaDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
        ;

        group.MapGet("empresa/{id}", GetEmpresaById)
            .WithName("GetById")
            .WithSummary("Obtiene una empresa por su ID")
            .Produces<EmpresaDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);


        group.MapGet("empresa/rfc/{rfc}", GetEmpresaByRfc)
            .WithName("GetEmpresaByRfc")
            .WithSummary("Obtiene una empresa por RFC")
            .Produces<EmpresaDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);


        group.MapGet("empresa/get-tiposdirecciones", GetListTipoDireccion)
            .WithName("GetTiposDirecciones")
            .WithSummary("Obtiene lista de tipo de direcciones")
            .Produces<IEnumerable<SelectItemDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);


        #endregion



    }

    #region Empresa
    public async Task<IResult> CreateEmpresa(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] EmpresaCreateDto model
    )
    {

        var command = new CreateEmpresaCommand(model);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateEmpresa(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] EmpresaUpdateDto model)
    {
        var command = new UpdateEmpresaCommand(id, model);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult(); // ✅ Maneja 200, 404, 400, 500
    }

    public async Task<IResult> DeleteEmpresa(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var command = new DeleteEmpresaCommand(id);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> EmpresaExists(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var query = new EmpresaExistsQuery(id);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetListTipoDireccion(
        [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetAllTiposDireccionesQuery();
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetEmpresaAll(
        [FromServices] IQueryMediator queryMediator
    )
    {
        var query = new GetAllEmpresasQuery();
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetEmpresaPaginated(
        IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var query = new GetEmpresasQuery(page, size, q);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetEmpresaById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var query = new GetEmpresaByIdQuery(id);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetEmpresaByRfc(
        [FromServices] IQueryMediator queryMediator,
        string rfc)
    {
        var query = new GetEmpresaByRfcQuery(rfc);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }
    #endregion




}

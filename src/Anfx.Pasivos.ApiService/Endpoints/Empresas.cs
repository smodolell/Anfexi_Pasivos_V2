using Anfx.Pasivos.Application.Features.Empresas.Commands;
using Anfx.Pasivos.Application.Features.Empresas.DTOs;
using Anfx.Pasivos.Application.Features.Empresas.Queries;
using Ardalis.Result.AspNetCore;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Empresas : EndpointGroupBase
{

    public override string? GroupName => "empresas";

    public override void Map(RouteGroupBuilder groupBuilder)
    {

        var group = groupBuilder.MapGroup("/")
            .RequireAuthorization()
            .WithTags("Empresas");


        group.MapPost("/", Create)
            .WithName("CreateEmpresa")
            .WithSummary("Crea una nueva empresa")
            .Accepts<EmpresaCreateDto>("application/json")
            .Produces<EmpresaCreateDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);


        group.MapPut("/{id}", Update)
            .WithName("UpdateEmpresa")
            .WithSummary("Actualiza una empresa existente")
            .Accepts<EmpresaUpdateDto>("application/json")
            .Produces<EmpresaDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", Delete)
            .WithName("DeleteEmpresa")
            .WithSummary("Elimina una empresa")
            .Produces<EmpresaDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("/", GetPaginated)
            .WithSummary("Obtiene empresas paginadas y filtradas")
            .Produces<PagedResultDto<EmpresaDto>>(StatusCodes.Status200OK);


        group.MapGet("/all", GetAll)
            .WithName("get-all")
            .WithSummary("Obtiene todas las empresas activas")
            .Produces<List<EmpresaDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
        ;

        group.MapGet("/{id}", GetById)
            .WithName("GetById")
            .WithSummary("Obtiene una empresa por su ID")
            .Produces<EmpresaDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);


        group.MapGet("/rfc/{rfc}", GetByRfc)
            .WithName("GetEmpresaByRfc")
            .WithSummary("Obtiene una empresa por RFC")
            .Produces<EmpresaDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);


        group.MapGet("/get-tiposdirecciones", GetListTipoDireccion)
            .WithName("GetTiposDirecciones")
            .WithSummary("Obtiene lista de tipo de direcciones")
            .Produces<IEnumerable<SelectItemDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);


    }




    public async Task<IResult> Create(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] EmpresaCreateDto model)
    {

        var command = new CreateEmpresaCommand(model);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> Update(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] EmpresaUpdateDto model)
    {
        var command = new UpdateEmpresaCommand(id, model);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult(); // ✅ Maneja 200, 404, 400, 500
    }

    public async Task<IResult> Delete(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var command = new DeleteEmpresaCommand(id);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> Exists(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var query = new EmpresaExistsQuery(id);
        var result = await queryMediator.QueryAsync(query);
        return result.ToMinimalApiResult();
    }

    public async Task<IResult> GetListTipoDireccion(
        [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetAllTiposDireccionesQuery();
        var result = await queryMediator.QueryAsync(query);
        return result.ToMinimalApiResult();
    }

    public async Task<IResult> GetAll(
        [FromServices] IQueryMediator queryMediator
    )
    {
        var query = new GetAllEmpresasQuery();
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetPaginated(
        IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var query = new GetEmpresasQuery(page, size, q);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var query = new GetEmpresaByIdQuery(id);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetByRfc(
        [FromServices] IQueryMediator queryMediator,
        string rfc)
    {
        var query = new GetEmpresaByRfcQuery(rfc);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }
}

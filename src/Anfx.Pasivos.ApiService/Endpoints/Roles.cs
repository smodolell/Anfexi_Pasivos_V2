using Anfx.Pasivos.ApiService.Requests;
using Anfx.Pasivos.Application.Features.Roles.Commands;
using Anfx.Pasivos.Application.Features.Roles.DTOs;
using Anfx.Pasivos.Application.Features.Roles.Queries;
using Anfx.Pasivos.Application.Features.Sistema.DTOs;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;


public class Roles : EndpointGroupBase
{
    public override string? GroupName => "roles";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            .WithTags("Roles");

        group.MapGet("/all", GetAll)
            .WithName("GetAllRoles")
            .WithSummary("Obtiene todos los roles activos")
            .WithDescription("Obtiene un listado completo de todos los roles activos")
            .Produces<ApiResponseDto<IEnumerable<RolDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("/paginados", GetPaginados)
            .WithName("GetRolesPaginados")
            .WithSummary("Obtiene roles paginados y filtrados")
            .WithDescription("Obtiene un listado paginado de roles con filtros opcionales")
            .Produces<ApiResponseDto<PagedResultDto<RolDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetById)
            .WithName("GetRolById")
            .WithSummary("Obtiene un rol por ID")
            .WithDescription("Obtiene los detalles de un rol específico por su ID")
            .Produces<ApiResponseDto<RolDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("/nombre/{nombre}", GetByName)
            .WithName("GetRolByName")
            .WithSummary("Obtiene un rol por nombre")
            .WithDescription("Obtiene los detalles de un rol específico por su nombre")
            .Produces<ApiResponseDto<RolDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}/exists", Exists)
            .WithName("RolExists")
            .WithSummary("Verifica si existe un rol")
            .WithDescription("Verifica si existe un rol con el ID especificado")
            .Produces<ApiResponseDto<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("/select-list", GetSelectList)
            .WithName("GetRolesSelectList")
            .WithSummary("Obtiene lista de roles para select")
            .WithDescription("Obtiene un listado de roles para controles tipo select")
            .Produces<ApiResponseDto<IEnumerable<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("/", Create)
            .WithName("CreateRol")
            .WithSummary("Crea un nuevo rol")
            .WithDescription("Crea un nuevo rol en el sistema")
            .Accepts<RolCreateDto>("application/json")
            .Produces<RolDto>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        // PUT /api/roles/{id} - Actualizar
        group.MapPut("/{id}", Update)
            .WithName("UpdateRol")
            .WithSummary("Actualiza un rol existente")
            .WithDescription("Actualiza los datos de un rol existente")
            .Accepts<RolUpdateDto>("application/json")
            .Produces<RolDto>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // DELETE /api/roles/{id} - Eliminar (soft delete)
        group.MapDelete("/{id}", Delete)
            .WithName("DeleteRol")
            .WithSummary("Elimina un rol")
            .WithDescription("Elimina lógicamente un rol (soft delete)")
            .Produces<bool>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    #region Handlers

    /// <summary>
    /// Obtiene todos los roles activos
    /// </summary>
    public async Task<IResult> GetAll(
        [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetRolesQuery(); // Nota: en tu controller usa GetAllRolesQuery, verifica cuál es el correcto
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginados(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {

        var query = new GetRolesQuery
        {
            Page = page,
            PageSize = size,
            SortColumn = "sRol",
            SortDescending = false,
            SearchTerm = q
        };

        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    /// <summary>
    /// Obtiene un rol por ID
    /// </summary>
    public async Task<IResult> GetById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var query = new GetRolByIdQuery(id);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    /// <summary>
    /// Obtiene un rol por nombre
    /// </summary>
    public async Task<IResult> GetByName(
        [FromServices] IQueryMediator queryMediator,
        string nombre)
    {
        var query = new GetRolByNameQuery(nombre);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    /// <summary>
    /// Verifica si existe un rol
    /// </summary>
    public async Task<IResult> Exists(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var query = new RolExistsQuery(id);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    /// <summary>
    /// Obtiene lista de roles para select
    /// </summary>
    public async Task<IResult> GetSelectList(
        [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetRolesSelectListQuery();
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    /// <summary>
    /// Crea un nuevo rol
    /// </summary>
    public async Task<IResult> Create(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] RolCreateDto model)
    {
        var command = new CreateRolCommand(model);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    /// <summary>
    /// Actualiza un rol existente
    /// </summary>
    public async Task<IResult> Update(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] RolUpdateDto model)
    {

        var command = new UpdateRolCommand(id, model);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    /// <summary>
    /// Elimina un rol (soft delete)
    /// </summary>
    public async Task<IResult> Delete(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var command = new DeleteRolCommand(id);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    #endregion
}
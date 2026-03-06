using Anfx.Pasivos.ApiService.Requests;
using Anfx.Pasivos.Application.Features.Roles.Commands;
using Anfx.Pasivos.Application.Features.Roles.DTOs;
using Anfx.Pasivos.Application.Features.Roles.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;


public class Roles : EndpointGroupBase
{
    public override string? GroupName => "roles";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            .WithTags("Roles");

        // GET /api/roles/all - Todos los roles activos
        group.MapGet("/all", GetAll)
            .WithName("GetAllRoles")
            .WithSummary("Obtiene todos los roles activos")
            .WithDescription("Obtiene un listado completo de todos los roles activos")
            .Produces<IEnumerable<RolDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /api/roles/paginados - Roles paginados (POST por el body)
        group.MapPost("/paginados", GetPaginados)
            .WithName("GetRolesPaginados")
            .WithSummary("Obtiene roles paginados y filtrados")
            .WithDescription("Obtiene un listado paginado de roles con filtros opcionales")
            .Accepts<GetPaginadosRequestDto>("application/json")
            .Produces<PagedResultDto<RolDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /api/roles/{id} - Por ID
        group.MapGet("/{id}", GetById)
            .WithName("GetRolById")
            .WithSummary("Obtiene un rol por ID")
            .WithDescription("Obtiene los detalles de un rol específico por su ID")
            .Produces<RolDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /api/roles/nombre/{nombre} - Por nombre
        group.MapGet("/nombre/{nombre}", GetByName)
            .WithName("GetRolByName")
            .WithSummary("Obtiene un rol por nombre")
            .WithDescription("Obtiene los detalles de un rol específico por su nombre")
            .Produces<RolDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /api/roles/{id}/exists - Verifica si existe
        group.MapGet("/{id}/exists", Exists)
            .WithName("RolExists")
            .WithSummary("Verifica si existe un rol")
            .WithDescription("Verifica si existe un rol con el ID especificado")
            .Produces<bool>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /api/roles/select-list - Lista para selects
        group.MapGet("/select-list", GetSelectList)
            .WithName("GetRolesSelectList")
            .WithSummary("Obtiene lista de roles para select")
            .WithDescription("Obtiene un listado de roles para controles tipo select")
            .Produces<IEnumerable<SelectItemDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /api/roles - Crear
        group.MapPost("/", Create)
            .WithName("CreateRol")
            .WithSummary("Crea un nuevo rol")
            .WithDescription("Crea un nuevo rol en el sistema")
            .Accepts<RolCreateDto>("application/json")
            .Produces<RolDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

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

    /// <summary>
    /// Obtiene roles paginados y filtrados (POST con body)
    /// </summary>
    public async Task<IResult> GetPaginados(
        [FromServices] IQueryMediator queryMediator,
        [FromBody] GetPaginadosRequestDto requestDto)
    {
        // Validación básica
        if (requestDto == null)
        {
            var validationError = Result<PagedResultDto<RolDto>>.Invalid(new List<ValidationError>
            {
                new() {
                    Identifier = "requestDto",
                    ErrorMessage = "Los parámetros de paginación son requeridos"
                }
            });
            return validationError.ToCustomMinimalApiResult();
        }

        var query = new GetRolesQuery
        {
            Page = requestDto.Page,
            PageSize = requestDto.Size,
            SortColumn = requestDto.SortColumn??"",
            SortDescending = requestDto.SortDescending
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
        // Validación de modelo nulo
        if (model == null)
        {
            var validationError = Result<RolDto>.Invalid(new List<ValidationError>
            {
                new() {
                    Identifier = "model",
                    ErrorMessage = "El modelo no puede ser nulo"
                }
            });
            return validationError.ToCustomMinimalApiResult();
        }

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

        var command = new UpdateRolCommand(id,model);
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
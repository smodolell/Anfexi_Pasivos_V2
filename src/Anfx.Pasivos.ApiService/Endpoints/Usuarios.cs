using Anfx.Pasivos.Application.Features.Usuarios.Commands;
using Anfx.Pasivos.Application.Features.Usuarios.DTOs;
using Anfx.Pasivos.Application.Features.Usuarios.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Usuarios : EndpointGroupBase
{
    public override string? GroupName => "usuarios";

    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
            .WithTags("Usuarios");

        // GET /api/usuarios/all - Todos los usuarios activos
        group.MapGet("/all", GetAll)
            .WithName("GetAllUsuarios")
            .WithSummary("Obtiene todos los usuarios activos")
            .WithDescription("Obtiene un listado completo de todos los usuarios activos")
            .Produces<IEnumerable<UsuarioDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /api/usuarios - Usuarios paginados
        group.MapGet("/", GetPaginados)
            .WithName("GetUsuariosPaginados")
            .WithSummary("Obtiene usuarios paginados y filtrados")
            .WithDescription("Obtiene un listado paginado de usuarios con filtros opcionales")
            .Produces<PagedResultDto<UsuarioDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /api/usuarios/{id} - Por ID
        group.MapGet("/{id}", GetById)
            .WithName("GetUsuarioById")
            .WithSummary("Obtiene un usuario por ID")
            .WithDescription("Obtiene los detalles de un usuario específico por su ID")
            .Produces<UsuarioDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /api/usuarios/roles - Lista de roles para selects
        group.MapGet("/roles", GetRoles)
            .WithName("GetRolesForUsuarios")
            .WithSummary("Obtiene lista de roles para selects")
            .WithDescription("Obtiene un listado de roles para controles tipo select en formularios de usuario")
            .Produces<IEnumerable<SelectItemDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /api/usuarios - Crear
        group.MapPost("/", Create)
            .WithName("CreateUsuario")
            .WithSummary("Crea un nuevo usuario")
            .WithDescription("Crea un nuevo usuario en el sistema")
            .Accepts<UsuarioCreateDto>("application/json")
            .Produces<UsuarioDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // PUT /api/usuarios/{id} - Actualizar
        group.MapPut("/{id}", Update)
            .WithName("UpdateUsuario")
            .WithSummary("Actualiza un usuario existente")
            .WithDescription("Actualiza los datos de un usuario existente")
            .Accepts<UsuarioUpdateDto>("application/json")
            .Produces<UsuarioDto>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // DELETE /api/usuarios/{id} - Eliminar (soft delete)
        group.MapDelete("/{id}", Delete)
            .WithName("DeleteUsuario")
            .WithSummary("Elimina un usuario")
            .WithDescription("Elimina lógicamente un usuario (soft delete)")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    #region Handlers

    /// <summary>
    /// Obtiene todos los usuarios activos
    /// </summary>
    public async Task<IResult> GetAll(
        [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetAllUsuariosQuery();
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetPaginados(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var query = new GetUsuariosQuery(page, size, q);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    
    public async Task<IResult> GetById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var query = new GetUsuarioByIdQuery(id);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetRoles(
        [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetRolesQuery();
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> Create(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] UsuarioCreateDto model)
    {
        // Validación de modelo nulo
        if (model == null)
        {
            var validationError = Result<UsuarioDto>.Invalid(new List<ValidationError>
            {
                new() {
                    Identifier = "model",
                    ErrorMessage = "El modelo no puede ser nulo"
                }
            });
            return validationError.ToCustomMinimalApiResult();
        }

        var command = new CreateUsuarioCommand(model);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    public async Task<IResult> Update(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] UsuarioUpdateDto model)
    {
        // Validaciones
        if (model == null)
        {
            var validationError = Result<UsuarioDto>.Invalid(new List<ValidationError>
            {
                new() {
                    Identifier = "model",
                    ErrorMessage = "El modelo no puede ser nulo"
                }
            });
            return validationError.ToCustomMinimalApiResult();
        }

        if (id != model.Id)
        {
            var validationError = Result<UsuarioDto>.Invalid(new List<ValidationError>
            {
                new() {
                    Identifier = "id",
                    ErrorMessage = $"El ID de la ruta ({id}) no coincide con el ID del usuario ({model.Id})"
                }
            });
            return validationError.ToCustomMinimalApiResult();
        }

        var command = new UpdateUsuarioCommand(model);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    /// <summary>
    /// Elimina un usuario (soft delete)
    /// </summary>
    public async Task<IResult> Delete(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var command = new DeleteUsuarioCommand(id);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    #endregion
}
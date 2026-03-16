using Anfx.Pasivos.Application.Features.Sistema.Commands;
using Anfx.Pasivos.Application.Features.Sistema.DTOs;
using Anfx.Pasivos.Application.Features.Sistema.Queries;
using Anfx.Pasivos.Application.Features.Usuarios.Commands;
using Anfx.Pasivos.Application.Features.Usuarios.DTOs;
using Anfx.Pasivos.Application.Features.Usuarios.Queries;
using Ardalis.Result.AspNetCore;
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


        group.MapPost("empresa/", Create)
            .WithName("CreateEmpresa")
            .WithSummary("Crea una nueva empresa")
            .Accepts<EmpresaCreateDto>("application/json")
            .Produces<EmpresaCreateDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);


        group.MapPut("empresa/{id}", Update)
            .WithName("UpdateEmpresa")
            .WithSummary("Actualiza una empresa existente")
            .Accepts<EmpresaUpdateDto>("application/json")
            .Produces<EmpresaDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapDelete("empresa/{id}", Delete)
            .WithName("DeleteEmpresa")
            .WithSummary("Elimina una empresa")
            .Produces<EmpresaDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("empresa/", GetPaginated)
            .WithSummary("Obtiene empresas paginadas y filtradas")
            .Produces<PagedResultDto<EmpresaDto>>(StatusCodes.Status200OK);


        group.MapGet("empresa/all", GetAll)
            .WithName("get-all")
            .WithSummary("Obtiene todas las empresas activas")
            .Produces<List<EmpresaDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
        ;

        group.MapGet("empresa/{id}", GetById)
            .WithName("GetById")
            .WithSummary("Obtiene una empresa por su ID")
            .Produces<EmpresaDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);


        group.MapGet("empresa/rfc/{rfc}", GetByRfc)
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


        group.MapGet("usuarios/all", GetUsuariosAll)
    .WithName("GetAllUsuarios")
    .WithSummary("Obtiene todos los usuarios activos")
    .WithDescription("Obtiene un listado completo de todos los usuarios activos")
    .Produces<IEnumerable<UsuarioDto>>(StatusCodes.Status200OK)
    .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
    .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("usuarios/", GetUsuariosPaginados)
            .WithName("GetUsuariosPaginados")
            .WithSummary("Obtiene usuarios paginados y filtrados")
            .WithDescription("Obtiene un listado paginado de usuarios con filtros opcionales")
            .Produces<PagedResultDto<UsuarioDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("usuarios/{id}", GetUsuarioById)
            .WithName("GetUsuarioById")
            .WithSummary("Obtiene un usuario por ID")
            .WithDescription("Obtiene los detalles de un usuario específico por su ID")
            .Produces<UsuarioDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("usuarios/roles", GetUsuarioRoles)
            .WithName("GetUsuarioRoles")
            .WithSummary("Obtiene lista de roles para selects")
            .WithDescription("Obtiene un listado de roles para controles tipo select en formularios de usuario")
            .Produces<IEnumerable<SelectItemDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("usuarios/", CreateUsuario)
            .WithName("CreateUsuario")
            .WithSummary("Crea un nuevo usuario")
            .WithDescription("Crea un nuevo usuario en el sistema")
            .Accepts<UsuarioCreateDto>("application/json")
            .Produces<UsuarioDto>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPut("usuarios/{id}", UpdateUsuario)
            .WithName("UpdateUsuario")
            .WithSummary("Actualiza un usuario existente")
            .WithDescription("Actualiza los datos de un usuario existente")
            .Accepts<UsuarioUpdateDto>("application/json")
            .Produces<UsuarioDto>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapDelete("usuarios/{id}", DeleteUsuario)
            .WithName("DeleteUsuario")
            .WithSummary("Elimina un usuario")
            .WithDescription("Elimina lógicamente un usuario (soft delete)")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }

    #region Empresa
    public async Task<IResult> Create(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] EmpresaCreateDto model
    )
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
    #endregion


    #region Usuarios

    public async Task<IResult> GetUsuariosAll(
        [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetAllUsuariosQuery();
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetUsuariosPaginados(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] bool? activo = null)
    {
        var query = new GetUsuariosQuery(page, size, q, activo);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }


    public async Task<IResult> GetUsuarioById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var query = new GetUsuarioByIdQuery(id);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetUsuarioRoles(
        [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetRolesQuery();
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateUsuario(
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
    public async Task<IResult> UpdateUsuario(
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
    public async Task<IResult> DeleteUsuario(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var command = new DeleteUsuarioCommand(id);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    #endregion

}

using Anfx.Pasivos.ApiService.Responces.Auth;
using Anfx.Pasivos.Application.Common.Interfaces;
using Anfx.Pasivos.Application.Features.Auth.Commands;
using Anfx.Pasivos.Application.Features.Auth.DTOs;
using Anfx.Pasivos.Application.Features.Auth.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Auth : EndpointGroupBase
{

    public override string? GroupName => "auth";
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
          .WithTags("Auth")
          .RequireAuthorization();


        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("Login por Correo Electronico")
            .WithDescription("Autentica un usuario con email y contraseña")
            .AllowAnonymous()
            .Accepts<LoginRequestDto>("application/json")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status409Conflict);


        group.MapPost("/login/username", LoginByUserName)
            .WithName("LoginByUserName")
            .AllowAnonymous()
            .WithSummary("Login por Nombre de Usuario")
            .WithDescription("Autentica un usuario con nombre de usuario y contraseña")
            .Accepts<LoginByUsernameRequestDto>("application/json")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError)
            .Produces<ApiResponseDto>(StatusCodes.Status409Conflict);

        group.MapPost("/validate-token", ValidateToken)
            .WithName("ValidateToken")
            .AllowAnonymous()
            .WithSummary("Valida un token JWT")
            .Accepts<LoginByUsernameRequestDto>("application/json")
            .Produces<Application.Features.Auth.DTOs.TokenValidationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);



        group.MapGet("me", GetCurrentUserProfile)
            .WithName("GetCurrentUserProfile")
            .WithSummary("Obtiene el perfil del usuario actual")
            .WithDescription("Retorna los datos del usuario extraídos del token JWT mediante UserContext")
            .Produces<ApiResponseDto<UserContextDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized);

        // group.MapPost("/refresh", RefreshToken)
        //     .WithName("RefreshToken")
        //     .WithSummary("Renueva el token JWT")
        //     .RequireAuthorization();

        #region Usuarios

        group.MapGet("usuarios/all", GetUsuariosAll)
            .WithName("GetAllUsuarios")
            .WithSummary("Obtiene todos los usuarios activos")
            .WithDescription("Obtiene un listado completo de todos los usuarios activos")
            .Produces<IEnumerable<UsuarioDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("usuarios/", GetUsuariosPaginados)
            .WithName("GetUsuariosPaginados")
            .WithSummary("Obtiene usuarios paginados y filtrados")
            .WithDescription("Obtiene un listado paginado de usuarios con filtros opcionales")
            .Produces<ApiResponseDto<PagedResultDto<UsuarioDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("usuarios/{id}", GetUsuarioById)
            .WithName("GetUsuarioById")
            .WithSummary("Obtiene un usuario por ID")
            .WithDescription("Obtiene los detalles de un usuario específico por su ID")
            .Produces<ApiResponseDto<UsuarioDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("usuarios/roles", GetUsuarioRoles)
            .WithName("GetUsuarioRoles")
            .WithSummary("Obtiene lista de roles para selects")
            .WithDescription("Obtiene un listado de roles para controles tipo select en formularios de usuario")
            .Produces<ApiResponseDto<IEnumerable<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("usuarios/", CreateUsuario)
            .WithName("CreateUsuario")
            .WithSummary("Crea un nuevo usuario")
            .WithDescription("Crea un nuevo usuario en el sistema")
            .Accepts<UsuarioCreateDto>("application/json")
            .Produces<ApiResponseDto<UsuarioDto>>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPut("usuarios/{id}", UpdateUsuario)
            .WithName("UpdateUsuario")
            .WithSummary("Actualiza un usuario existente")
            .WithDescription("Actualiza los datos de un usuario existente")
            .Accepts<UsuarioUpdateDto>("application/json")
            .Produces<ApiResponseDto<UsuarioDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapDelete("usuarios/{id}", DeleteUsuario)
            .WithName("DeleteUsuario")
            .WithSummary("Elimina un usuario")
            .WithDescription("Elimina lógicamente un usuario (soft delete)")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        #endregion

        #region Roles

        group.MapGet("rol/all", GetRolAll)
            .WithName("GetAllRoles")
            .WithSummary("Obtiene todos los roles activos")
            .WithDescription("Obtiene un listado completo de todos los roles activos")
            .Produces<ApiResponseDto<IEnumerable<RolDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("rol/paginados", GetRolPaginados)
            .WithName("GetRolesPaginados")
            .WithSummary("Obtiene roles paginados y filtrados")
            .WithDescription("Obtiene un listado paginado de roles con filtros opcionales")
            .Produces<ApiResponseDto<PagedResultDto<RolDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("rol/{id}", GetRolById)
            .WithName("GetRolById")
            .WithSummary("Obtiene un rol por ID")
            .WithDescription("Obtiene los detalles de un rol específico por su ID")
            .Produces<ApiResponseDto<RolDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("rol/nombre/{nombre}", GetRolByName)
            .WithName("GetRolByName")
            .WithSummary("Obtiene un rol por nombre")
            .WithDescription("Obtiene los detalles de un rol específico por su nombre")
            .Produces<ApiResponseDto<RolDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("rol/{id}/exists", RolExists)
            .WithName("RolExists")
            .WithSummary("Verifica si existe un rol")
            .WithDescription("Verifica si existe un rol con el ID especificado")
            .Produces<ApiResponseDto<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapGet("rol/select-list", GetRolSelectList)
            .WithName("GetRolesSelectList")
            .WithSummary("Obtiene lista de roles para select")
            .WithDescription("Obtiene un listado de roles para controles tipo select")
            .Produces<ApiResponseDto<IEnumerable<SelectItemDto>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);

        group.MapPost("rol/", CreateRol)
            .WithName("CreateRol")
            .WithSummary("Crea un nuevo rol")
            .WithDescription("Crea un nuevo rol en el sistema")
            .Accepts<RolCreateDto>("application/json")
            .Produces<ApiResponseDto<RolDto>>(StatusCodes.Status201Created)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapPut("rol/{id}", UpdateRol)
            .WithName("UpdateRol")
            .WithSummary("Actualiza un rol existente")
            .WithDescription("Actualiza los datos de un rol existente")
            .Accepts<RolUpdateDto>("application/json")
            .Produces<ApiResponseDto<RolDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        group.MapDelete("rol/{id}", DeleteRol)
            .WithName("DeleteRol")
            .WithSummary("Elimina un rol")
            .WithDescription("Elimina lógicamente un rol (soft delete)")
            .Produces<ApiResponseDto<bool>>(StatusCodes.Status200OK)
            .Produces<ApiResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseDto>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseDto>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseDto>(StatusCodes.Status500InternalServerError);


        #endregion
    }


    #region Login
    public async Task<IResult> Login(
  [FromServices] ICommandMediator commandMediator,
  [FromBody] LoginRequestDto model)
    {

        var command = new LoginCommand
        {
            Email = model.Email,
            Usuario = string.Empty,
            Contrasenia = model.Contrasenia
        };

        var result = await commandMediator.SendAsync(command);
        if (result.IsSuccess)
        {
            var r = new LoginResponse(true, new UserInfo(result.Value.Id)
            {
                NombreCompleto = result.Value.NombreCompleto,
                Email = result.Value.Email,
                UsuarioNombre = result.Value.UsuarioNombre,
                Role = result.Value.Role
            })
            {
                Message = "Login exitoso",
                Token = result.Value.Token
            };
            return Results.Ok(r);
        }

        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> LoginByUserName(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] LoginByUsernameRequestDto model)
    {

        var command = new LoginCommand
        {
            Email = string.Empty,
            Usuario = model.UsuarioNombre,
            Contrasenia = model.Contrasena
        };

        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> ValidateToken(
        [FromServices] IQueryMediator queryMediator,
        [FromServices] string token
        )
    {
        var query = new ValidateTokenQuery(token);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetCurrentUserProfile(
        [FromServices] IUserContext userContext)
    {
        if (!userContext.IsAuthenticated)
        {

            return Result.Unauthorized("Usuario no autenticado").
                ToCustomMinimalApiResult();
        }

        var userDto = new UserContextDto
        {
            Id = userContext.UserId,
            Username = userContext.UserName,
            Email = userContext.Email,
            Role = userContext.Role,
            IsAuthenticated = userContext.IsAuthenticated
        };
        return Result.Success(userDto, "Perfil recuperado exitosamente")
                .ToCustomMinimalApiResult();
    }

    #endregion

    #region Roles

    /// <summary>
    /// Obtiene todos los roles activos
    /// </summary>
    public async Task<IResult> GetRolAll(
        [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetRolesQuery(); // Nota: en tu controller usa GetAllRolesQuery, verifica cuál es el correcto
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetRolPaginados(
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

    public async Task<IResult> GetRolById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var query = new GetRolByIdQuery(id);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetRolByName(
        [FromServices] IQueryMediator queryMediator,
        string nombre)
    {
        var query = new GetRolByNameQuery(nombre);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> RolExists(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var query = new RolExistsQuery(id);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> GetRolSelectList(
        [FromServices] IQueryMediator queryMediator)
    {
        var query = new GetRolesSelectListQuery();
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> CreateRol(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] RolCreateDto model)
    {
        var command = new CreateRolCommand(model);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> UpdateRol(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] RolUpdateDto model)
    {

        var command = new UpdateRolCommand(id, model);
        var result = await commandMediator.SendAsync(command);
        return result.ToCustomMinimalApiResult();
    }

    public async Task<IResult> DeleteRol(
        [FromServices] ICommandMediator commandMediator,
        int id)
    {
        var command = new DeleteRolCommand(id);
        var result = await commandMediator.SendAsync(command);
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

    public static async Task<IResult> GetUsuariosPaginados(
        [FromServices] IQueryMediator queryMediator,
        [FromQuery] string? q = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] bool? activo = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = null)
    {
        var query = new GetUsuariosQuery(page, size, q, activo, sortBy, sortDir);
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

using Anfx.Pasivos.ApiService.Responces.Auth;
using Anfx.Pasivos.Application.Features.Auth.Commands;
using Anfx.Pasivos.Application.Features.Auth.Queries;
using Anfx.Pasivos.Application.Features.Usuarios.DTOs;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Anfx.Pasivos.ApiService.Endpoints;

public class Auth : EndpointGroupBase
{

    public override string? GroupName => "auth";
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/")
          .WithTags("Auth");


        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("Login por Correo Electronico")
            .WithDescription("Autentica un usuario con email y contraseña")
            .Accepts<LoginRequestDto>("application/json")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);


        group.MapPost("/login/username", LoginByUserName)
            .WithName("LoginByUserName")
            .WithSummary("Login por Nombre de Usuario")
            .WithDescription("Autentica un usuario con nombre de usuario y contraseña")
            .Accepts<LoginByUsernameRequestDto>("application/json")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ValidationProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ValidationProblemDetails>(StatusCodes.Status500InternalServerError)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);



        group.MapPost("/validate-token", ValidateToken)
            .WithSummary("Valida un token JWT")
            .Accepts<LoginByUsernameRequestDto>("application/json")
            .Produces<Application.Features.Auth.DTOs.TokenValidationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }


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
        if(result.IsSuccess)
        {
            var r = new LoginResponse(true,new UserInfo(result.Value.Id)
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

    public static async Task<IResult> ValidateToken(
        [FromServices] IQueryMediator queryMediator,
        [FromServices] string token
        )
    {
        var query = new ValidateTokenQuery(token);
        var result = await queryMediator.QueryAsync(query);
        return result.ToCustomMinimalApiResult();
    }
}

using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Features.Auth.Queries;

public record ValidateTokenQuery(string Token) : IQuery<Result<TokenValidationDto>>;

public class ValidateTokenQueryHandler : IQueryHandler<ValidateTokenQuery, Result<TokenValidationDto>>
{
    private readonly IJwtService _jwtService;

    public ValidateTokenQueryHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

  
    public async Task<Result<TokenValidationDto>> HandleAsync(ValidateTokenQuery request, CancellationToken cancellationToken = default)
    {
        var validationResult = _jwtService.ValidateToken(request.Token);
        if (!validationResult)
        {;
            return await Task.FromResult(Result.Invalid(new ValidationError("Token inválido o expirado")));
        }

        var user = _jwtService.GetUserFromToken(request.Token);
        if (user == null)
        {
            return await Task.FromResult(Result.Invalid(new ValidationError("Usuario no encontrado en el token")));
        }

        return await Task.FromResult(Result.Success(new TokenValidationDto
        {
            IsValid = true,
            User = user
        }));
    }
}


using Anfx.Pasivos.Application.Common.Interfaces;
using Anfx.Pasivos.Application.Features.Auth.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Anfx.Pasivos.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(UsuarioLoginDto user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? "ProfuturoSecretKey2024");
        var issuer = jwtSettings["Issuer"] ?? "ProfuturoAPI";
        var audience = jwtSettings["Audience"] ?? "ProfuturoUsers";
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UsuarioNombre),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.NombreCompleto),
                new Claim(ClaimTypes.GivenName, user.Role),
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? "ProfuturoSecretKey2024");
        var issuer = jwtSettings["Issuer"] ?? "ProfuturoAPI";
        var audience = jwtSettings["Audience"] ?? "ProfuturoUsers";

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public UsuarioLoginDto? GetUserFromToken(string token)
    {
        var validationResult = ValidateToken(token);
        if (!validationResult)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        var userNameClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
        var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        var fullNameClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);

        if (userIdClaim == null || userNameClaim == null || emailClaim == null)
            return null;

        // El rol está en el segundo ClaimTypes.GivenName (índice 1)
        var roleClaims = jwtToken.Claims.Where(x => x.Type == ClaimTypes.GivenName).ToList();
        var role = roleClaims.Count > 1 ? roleClaims[1].Value : string.Empty;
        var nombreCompleto = roleClaims.Count > 0 ? roleClaims[0].Value : string.Empty;

        return new UsuarioLoginDto
        {
            Id = int.Parse(userIdClaim.Value),
            UsuarioNombre = userNameClaim.Value,
            Email = emailClaim.Value,
            NombreCompleto = nombreCompleto,
            Role = role,
            Token = token,
            RefreshToken = string.Empty,
            TokenExpiration = jwtToken.ValidTo
        };
    }
}
namespace Anfx.Pasivos.ApiService.Infrastructure;

public sealed class OktaSettings
{
    public const string SectionName = "Okta";

    /// <summary>Ej: https://dev-XXXXXXXX.okta.com</summary>
    public string Domain { get; init; } = string.Empty;

    /// <summary>ID del Authorization Server. "default" para el servidor por defecto.</summary>
    public string AuthorizationServerId { get; init; } = "default";

    /// <summary>Audience configurada en el Authorization Server de Okta. Ej: api://default</summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>Claim de Okta que contiene los roles/grupos. Por defecto "groups".</summary>
    public string RoleClaimType { get; init; } = "groups";

    /// <summary>URL completa del issuer: Domain + /oauth2/ + AuthorizationServerId</summary>
    public string Authority => $"{Domain.TrimEnd('/')}/oauth2/{AuthorizationServerId}";
}

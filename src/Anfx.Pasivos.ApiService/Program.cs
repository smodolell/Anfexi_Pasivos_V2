using Anfx.Pasivos.ApiService;
using Anfx.Pasivos.ApiService.Infrastructure;
using Anfx.Pasivos.Application;
using Anfx.Pasivos.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);

builder.AddWebServices();
builder.Services.AddControllers();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["server"] = Environment.MachineName;
    };
});

// ── Okta JWT Authentication ────────────────────────────────────────────────
var oktaSettings = builder.Configuration
    .GetSection(OktaSettings.SectionName)
    .Get<OktaSettings>()
    ?? throw new InvalidOperationException("Falta la sección 'Okta' en appsettings.json");

builder.Services.Configure<OktaSettings>(
    builder.Configuration.GetSection(OktaSettings.SectionName));

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Authority descarga automáticamente el JWKS desde:
    // {Authority}/.well-known/openid-configuration  →  keys_uri
    options.Authority    = oktaSettings.Authority;
    options.Audience     = oktaSettings.Audience;
    options.MetadataAddress = $"{oktaSettings.Authority}/.well-known/oauth-authorization-server";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidIssuer              = oktaSettings.Authority,
        ValidateAudience         = true,
        ValidAudience            = oktaSettings.Audience,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        // Okta usa RS256 – la clave pública se obtiene del JWKS automáticamente
        // RoleClaimType mapea el claim de grupos de Okta a los roles de ASP.NET Core
        RoleClaimType            = oktaSettings.RoleClaimType,
    };

    options.Events = new JwtBearerEvents
    {
        // Logging de intentos fallidos de autenticación
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();

            logger.LogWarning(
                "Autenticación fallida | IP: {IP} | Path: {Path} | Error: {Error}",
                context.HttpContext.Connection.RemoteIpAddress,
                context.HttpContext.Request.Path,
                context.Exception.Message);

            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();

            var userId = context.Principal?.FindFirst("sub")?.Value;
            logger.LogInformation(
                "Token válido | UserId: {UserId} | Path: {Path}",
                userId,
                context.HttpContext.Request.Path);

            return Task.CompletedTask;
        },

        // 401 con cuerpo JSON consistente
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode      = StatusCodes.Status401Unauthorized;
            context.Response.ContentType     = "application/json";

            await context.Response.WriteAsJsonAsync(new ApiResponseDto<object>
            {
                Success    = false,
                Message    = "No autorizado. El token es inválido, ha expirado o no fue enviado.",
                StatusCode = StatusCodes.Status401Unauthorized,
                Timestamp  = DateTime.UtcNow,
                TraceId    = context.HttpContext.TraceIdentifier
            });
        },

        // 403 con cuerpo JSON consistente
        OnForbidden = async context =>
        {
            context.Response.StatusCode  = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new ApiResponseDto<object>
            {
                Success    = false,
                Message    = "Acceso denegado. No tiene los permisos necesarios para este recurso.",
                StatusCode = StatusCodes.Status403Forbidden,
                Timestamp  = DateTime.UtcNow,
                TraceId    = context.HttpContext.TraceIdentifier
            });
        }
    };
});

// ── Authorization Policies ────────────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    // Política base: solo requiere autenticación
    options.AddPolicy("Authenticated", policy =>
        policy.RequireAuthenticatedUser());

    // Administradores del sistema
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Gerencia o Administración
    options.AddPolicy("GerenciaOrAdmin", policy =>
        policy.RequireRole("Admin", "Gerencia"));

    // Solo operadores (captura de contratos, etc.)
    options.AddPolicy("OperadoresOnly", policy =>
        policy.RequireRole("Operador", "Admin"));

    // Reportes: cualquier usuario autenticado con rol de lectura
    options.AddPolicy("Reportes", policy =>
        policy.RequireRole("Admin", "Gerencia", "Operador", "Consulta"));
});

// Configuración de CORS
// Los orígenes adicionales de producción se pueden agregar vía appsettings.Production.json:
//   "AllowedOrigins": ["http://dev.anfexi.com", "https://dev.anfexi.com"]
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration
        .GetSection("AllowedOrigins")
        .Get<string[]>()
        ?? Array.Empty<string>();

    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
var app = builder.Build();
//app.UseExceptionHandler(options => { });
app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();

    //app.MapScalarApiReference(options =>
    //{
    //    options.WithTitle("Yggdrasil API Documentation");
    //    options.WithTheme(ScalarTheme.Saturn);
    //    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    //    options.HideSearch = true;// Habilita/Deshabilita el buscador (Ctrl+K)
    //    options.ShowSidebar = true; // Muestra u oculta la barra lateral
    //    options.DarkMode = true;
    //});
}

#if (!UseAspire)
app.UseHealthChecks("/health");
#endif


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();

// Swagger y Scalar solo en desarrollo — no exponer en producción
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Pasivos V1");
    });
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("API Services Catalogos");
        options.WithTheme(ScalarTheme.DeepSpace);
        options.WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.AsyncHttp);
        options.HideSearch = true;
        options.ShowSidebar = true;
        options.DarkMode = false;
    });

    app.Map("/", () => Results.Redirect("/scalar"));
}


#if (UseAspire)
app.MapDefaultEndpoints();
#endif

app.MapEndpoints();

app.MapControllers();

app.Run();

using Anfx.Pasivos.Infrastructure.Persistence;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using System.Reflection;

namespace Anfx.Pasivos.ApiService;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddHttpContextAccessor();

#if (!UseAspire)
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();
#endif


        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Error de validación en los parámetros",
                    Detail = "Uno o más parámetros no pudieron ser convertidos al tipo esperado."
                };
                return new BadRequestObjectResult(problemDetails);
            };
            options.SuppressModelStateInvalidFilter = true;
        });
        builder.Services.AddEndpointsApiExplorer();

        // CONFIGURACIÓN OPENAPI PARA SCALAR
        builder.Services.AddOpenApi(options =>
        {
            // Transformer 1: Seguridad JWT
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                const string schemeName = "Bearer";
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                var securityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Token de autenticación JWT"
                };

                document.Components.SecuritySchemes.TryAdd(schemeName, securityScheme);

                var reference = new OpenApiSecuritySchemeReference(schemeName);
                var requirement = new OpenApiSecurityRequirement { [reference] = [] };

                if (document.Paths != null)
                {
                    foreach (var operation in document.Paths.Values.SelectMany(v => v.Operations.Values))
                    {
                        operation.Security ??= new List<OpenApiSecurityRequirement>();
                        operation.Security.Add(requirement);
                    }
                }
                return Task.CompletedTask;
            });

            // Transformer 2: Info de la API (Separado, no anidado)
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Contact = new() { Name = "Soporte Técnico", Email = "anfexi@soporte.com" };
                document.Info.License = new() { Name = "MIT" };
                return Task.CompletedTask;
            });
        });

        // SwaggerGen se mantiene solo si lo usas para generar los XMLs, 
        // pero Scalar usa lo configurado en AddOpenApi.
        builder.Services.AddSwaggerGen(c =>
        {
            var apiXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, apiXml));

            c.SchemaFilter<ApiResponseSchemaFilter>();
        });

        builder.Services.AddFluentValidationRulesToSwagger();

    }
    public static void AddWebServices2(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddHttpContextAccessor();

#if (!UseAspire)
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();
#endif


        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
            const string schemeName = "Bearer";

            // 1. Asegurar que Components y SecuritySchemes existan
            document.Components ??= new OpenApiComponents();

            var securityScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Token de autenticación JWT"
            };

            // Usamos TryAdd para evitar errores si ya existe
            document.Components.SecuritySchemes.TryAdd(schemeName, securityScheme);

            // 2. Crear la referencia correctamente para .NET 10
            var reference = new OpenApiSecuritySchemeReference(schemeName);

            var requirement = new OpenApiSecurityRequirement
            {
                [reference] = []
            };

                // 3. Aplicar seguridad a las operaciones de forma segura
                if (document.Paths != null)
                {
                    foreach (var path in document.Paths.Values)
                    {
                        foreach (var operation in path.Operations.Values)
                        {
                            operation.Security ??= new List<OpenApiSecurityRequirement>();
                            operation.Security.Add(requirement);
                        }
                    } 
                }

                options.AddDocumentTransformer((document, context, cancellationToken) =>
                          {
                              document.Info.Contact = new() { Name = "Soporte Técnico", Email = "anfexi@soporte.com" };
                              document.Info.License = new() { Name = "MIT" };
                              return Task.CompletedTask;
                          });
                return Task.CompletedTask;
            });
        });

        builder.Services.AddSwaggerGen(c =>
        {


            // 1. Documentación de la propia API
            var apiXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, apiXml));

            // 2. Documentación de la Capa de Aplicación
            // Usamos CreateInterestRateCommand para encontrar la ruta del ensamblado de Aplicación
            var appXml = $"{typeof(Application.DependencyInjection).Assembly.GetName().Name}.xml";
            var appPath = Path.Combine(AppContext.BaseDirectory, appXml);

            if (File.Exists(appPath))
            {
                c.IncludeXmlComments(appPath);
            }
        });

    }

}

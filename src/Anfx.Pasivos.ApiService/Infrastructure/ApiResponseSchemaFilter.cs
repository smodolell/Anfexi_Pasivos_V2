using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Anfx.Pasivos.ApiService.Infrastructure;

public class ApiResponseSchemaFilter : ISchemaFilter
{

    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsGenericType && context.Type.GetGenericTypeDefinition() == typeof(ApiResponseDto<>))
        {
            var innerType = context.Type.GetGenericArguments()[0];

            // Generar el esquema para el tipo T
            var innerSchema = context.SchemaGenerator.GenerateSchema(innerType, context.SchemaRepository);

            // Agregar la propiedad "data" al wrapper
            schema.Properties["data"] = innerSchema;
        }
    }
}

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Pavis.WebApi.Swagger;

/// <summary>
/// Filter personalizado para corregir el tipo de la propiedad Role en Swagger
/// Fuerza que se muestre como string en lugar de enum numérico
/// </summary>
public class RolePropertySchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Buscar propiedades con nombre "Role" o "role"
        if (schema.Properties != null)
        {
            foreach (var prop in schema.Properties.Where(p => p.Key.Equals("Role", StringComparison.OrdinalIgnoreCase)))
            {
                // Reemplazar el esquema de referencia por un simple string
                schema.Properties[prop.Key] = new OpenApiSchema
                {
                    Type = "string",
                    Description = prop.Value.Description ?? "Rol del usuario (enviar como string/texto)",
                    Example = new Microsoft.OpenApi.Any.OpenApiString("ADMIN"),
                    Enum = new List<Microsoft.OpenApi.Any.IOpenApiAny>
                    {
                        new Microsoft.OpenApi.Any.OpenApiString("ADMIN"),
                        new Microsoft.OpenApi.Any.OpenApiString("ASESOR"),
                        new Microsoft.OpenApi.Any.OpenApiString("SPAT"),
                        new Microsoft.OpenApi.Any.OpenApiString("CONSULTA"),
                        new Microsoft.OpenApi.Any.OpenApiString("ORGANIZACION")
                    }
                };
            }
        }
    }
}

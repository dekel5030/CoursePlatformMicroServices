using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoursePlatform.ServiceDefaults.Swagger;

public sealed class ProblemDetailsOperationFilter(string securitySchemeId) : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        AddResponse(operation, context, StatusCodes.Status400BadRequest, "Bad Request");
        AddResponse(operation, context, StatusCodes.Status404NotFound, "Not Found");
        AddResponse(operation, context, StatusCodes.Status409Conflict, "Conflict");
        AddResponse(operation, context, StatusCodes.Status500InternalServerError, "Server Error");

        var authMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .OfType<Microsoft.AspNetCore.Authorization.IAuthorizeData>();

        var allowAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .Any(m => m is Microsoft.AspNetCore.Authorization.IAllowAnonymous);

        if (authMetadata.Any() && !allowAnonymous)
        {
            AddResponse(operation, context, StatusCodes.Status401Unauthorized, "Unauthorized");

            var schemeReference = new OpenApiSecuritySchemeReference(securitySchemeId, null, null);

            operation.Security =
            [
                new OpenApiSecurityRequirement
                {
                    [schemeReference] = []
                }
            ];
        }
    }

    private static void AddResponse(OpenApiOperation operation, OperationFilterContext context, int statusCode, string description)
    {
        if (operation?.Responses == null ||
            context?.SchemaGenerator == null ||
            context.SchemaRepository == null)
        {
            return;
        }

        var code = statusCode.ToString(CultureInfo.InvariantCulture);

        if (!operation.Responses.ContainsKey(code))
        {
            operation.Responses.Add(code, new OpenApiResponse
            {
                Description = description,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/problem+json"] = new()
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
                    }
                }
            });
        }
    }
}
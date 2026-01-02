using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoursePlatform.ServiceDefaults.Swagger;

public class ProblemDetailsOperationFilter : IOperationFilter
{
    private readonly string _securitySchemeId;

    public ProblemDetailsOperationFilter(string securitySchemeId)
    {
        _securitySchemeId = securitySchemeId;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        AddResponse(operation, context, StatusCodes.Status400BadRequest, "Bad Request (Validation or Logic Error)");
        AddResponse(operation, context, StatusCodes.Status404NotFound, "The requested resource was not found");
        AddResponse(operation, context, StatusCodes.Status409Conflict, "A business conflict occurred");
        AddResponse(operation, context, StatusCodes.Status500InternalServerError, "An unexpected server error occurred");

        var authMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .OfType<Microsoft.AspNetCore.Authorization.IAuthorizeData>();

        var allowAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .Any(m => m is Microsoft.AspNetCore.Authorization.IAllowAnonymous);

        if (authMetadata.Any() && !allowAnonymous)
        {
            AddResponse(operation, context, StatusCodes.Status401Unauthorized, "Unauthorized access");

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = _securitySchemeId
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            };
        }
    }

    private static void AddResponse(OpenApiOperation operation, OperationFilterContext context, int statusCode, string description)
    {
        var code = statusCode.ToString();
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
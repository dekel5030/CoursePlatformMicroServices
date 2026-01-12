using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CoursePlatform.ServiceDefaults.Swagger;

public static class EndpointExtensions
{
    public static RouteHandlerBuilder WithMetadata<TResponse>(
        this RouteHandlerBuilder builder,
        string endpointName,
        string tag,
        string summary,
        int successStatusCode = 200)
    {
        RouteHandlerBuilder result = builder
            .WithName(endpointName)
            .WithTags(tag)
            .WithSummary(summary)
            .Produces<TResponse>(successStatusCode);

        return result;
    }
}

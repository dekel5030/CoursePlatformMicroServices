using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Api.Infrastructure.LinkProvider;

internal sealed class HttpLinkResolver : IHttpLinkResolver
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpLinkResolver(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetHref(string endpointName, object? routeValues)
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HTTP context is not available.");

        string? href = _linkGenerator.GetUriByName(httpContext, endpointName, routeValues);

        return href ?? throw new InvalidOperationException(
            $"Could not generate URL for endpoint '{endpointName}'.");
    }
}

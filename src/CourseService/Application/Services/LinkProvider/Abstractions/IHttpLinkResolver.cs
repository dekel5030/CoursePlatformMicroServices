namespace Courses.Application.Services.LinkProvider.Abstractions;

public interface IHttpLinkResolver
{
    string GetHref(string endpointName, object? routeValues);
}

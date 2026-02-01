namespace Courses.Application.Services.LinkProvider.Abstractions;

internal interface ILinkDefinition
{
    string Rel { get; }
    string Method { get; }
    string EndpointName { get; }
    bool IsAllowed(object context);
    object? GetRouteValues(object context);
}

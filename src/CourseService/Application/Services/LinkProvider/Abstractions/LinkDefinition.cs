namespace Courses.Application.Services.LinkProvider.Abstractions;

internal sealed class LinkDefinition<TContext> : ILinkDefinition
{
    public string Rel { get; }
    public string Method { get; }
    public string EndpointName { get; }
    private readonly Func<TContext, bool> _policyCheck;
    private readonly Func<TContext, object?> _getRouteValues;

    public LinkDefinition(
        string rel,
        string method,
        string endpointName,
        Func<TContext, bool> policyCheck,
        Func<TContext, object?> getRouteValues)
    {
        Rel = rel;
        Method = method;
        EndpointName = endpointName;
        _policyCheck = policyCheck;
        _getRouteValues = getRouteValues;
    }

    public bool IsAllowed(object context) =>
        context is TContext ctx && _policyCheck(ctx);

    public object? GetRouteValues(object context) =>
        context is TContext ctx ? _getRouteValues(ctx) : null;
}

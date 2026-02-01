using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Services.LinkProvider;

internal sealed class LinkBuilderService : ILinkBuilderService
{
    private readonly Dictionary<LinkResourceKey, IReadOnlyList<ILinkDefinition>> _definitionsByResource;
    private readonly IHttpLinkResolver _resolver;

    public LinkBuilderService(
        IEnumerable<ILinkDefinitionRegistry> registries,
        IHttpLinkResolver resolver)
    {
        _definitionsByResource = registries.ToDictionary(r => r.ResourceKey, r => r.GetDefinitions());
        _resolver = resolver;
    }

    public IReadOnlyList<LinkDto> BuildLinks<TContext>(LinkResourceKey resourceKey, TContext context)
    {
        if (!_definitionsByResource.TryGetValue(resourceKey, out IReadOnlyList<ILinkDefinition>? definitions))
        {
            return Array.Empty<LinkDto>();
        }

        var links = new List<LinkDto>();
        foreach (ILinkDefinition definition in definitions)
        {
            if (!definition.IsAllowed(context!))
            {
                continue;
            }

            object? routeValues = definition.GetRouteValues(context!);
            string href = _resolver.GetHref(definition.EndpointName, routeValues);
            links.Add(new LinkDto
            {
                Href = href,
                Rel = definition.Rel,
                Method = definition.Method
            });
        }

        return links.AsReadOnly();
    }
}

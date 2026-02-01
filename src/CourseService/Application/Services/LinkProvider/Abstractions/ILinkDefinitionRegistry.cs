namespace Courses.Application.Services.LinkProvider.Abstractions;

internal interface ILinkDefinitionRegistry
{
    LinkResourceKey ResourceKey { get; }
    IReadOnlyList<ILinkDefinition> GetDefinitions();
}

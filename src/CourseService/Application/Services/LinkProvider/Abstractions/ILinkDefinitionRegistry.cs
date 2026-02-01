namespace Courses.Application.Services.LinkProvider.Abstractions;

internal interface ILinkDefinitionRegistry
{
    string ResourceKey { get; }
    IReadOnlyList<ILinkDefinition> GetDefinitions();
}

namespace Courses.Application.Services.LinkProvider.Abstractions;

/// <summary>
/// Legacy link builder returning List of LinkDto. Prefer ILinkProvider with strongly-typed link records per feature.
/// Still used by GetCourseById (Courses) and by shared mappers until those are removed or refactored.
/// </summary>
public interface ILinkBuilderService
{
    IReadOnlyList<LinkDto> BuildLinks<TContext>(LinkResourceKey resourceKey, TContext context);
}

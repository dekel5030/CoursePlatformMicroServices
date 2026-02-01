namespace Courses.Application.Services.LinkProvider.Abstractions;

public interface ILinkBuilderService
{
    IReadOnlyList<LinkDto> BuildLinks<TContext>(LinkResourceKey resourceKey, TContext context);
}

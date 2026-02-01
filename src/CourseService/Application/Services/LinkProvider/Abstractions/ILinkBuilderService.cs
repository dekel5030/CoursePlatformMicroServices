namespace Courses.Application.Services.LinkProvider.Abstractions;

public interface ILinkBuilderService
{
    IReadOnlyList<LinkDto> BuildLinks<TContext>(string resourceKey, TContext context);
}

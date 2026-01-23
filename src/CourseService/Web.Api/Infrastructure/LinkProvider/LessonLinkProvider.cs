using Courses.Api.Infrastructure.LinkProvider.Abstractions;
using Courses.Application.Abstractions.LinkProvider;
using Courses.Application.Actions.Lessons;

namespace Courses.Api.Infrastructure.LinkProvider;

internal sealed class LessonLinkProvider : LinkProviderBase, ILessonLinkProvider
{
    private readonly ILessonActionProvider _lessonActionProvider;

    public LessonLinkProvider(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor,
        ILessonActionProvider lessonActionProvider) : base(linkGenerator, httpContextAccessor)
    {
        _lessonActionProvider = lessonActionProvider;
    }

    public IReadOnlyCollection<LinkDto> CreateLinks(LessonState state)
    {
        var allowed = _lessonActionProvider.GetAllowedActions(state).ToHashSet();
        var links = new List<LinkDto>();
        
        if (allowed is null)
        {
            return links;
        }

        return links;
    }
}

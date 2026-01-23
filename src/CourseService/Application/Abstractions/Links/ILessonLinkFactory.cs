using Courses.Application.Abstractions.LinkProvider;
using Courses.Domain.Lessons;

namespace Courses.Application.Abstractions.Links;

public interface ILessonLinkFactory
{
    IReadOnlyList<LinkDto> CreateLinks(Lesson lesson);
}

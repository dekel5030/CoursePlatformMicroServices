using Courses.Domain.Lessons;

namespace Courses.Application.Services.LinkProvider.Abstractions.Factories;

public interface ILessonLinkFactory
{
    IReadOnlyList<LinkDto> CreateLinks(Lesson lesson);
}

using Courses.Domain.Courses;

namespace Courses.Application.Services.LinkProvider.Abstractions.Factories;

public interface ICourseLinkFactory
{
    IReadOnlyList<LinkDto> CreateLinks(Course course);
}

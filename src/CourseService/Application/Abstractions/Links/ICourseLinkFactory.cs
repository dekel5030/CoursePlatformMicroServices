using Courses.Application.Abstractions.LinkProvider;
using Courses.Domain.Courses;

namespace Courses.Application.Abstractions.Links;

public interface ICourseLinkFactory
{
    IReadOnlyList<LinkDto> CreateLinks(Course course);
}

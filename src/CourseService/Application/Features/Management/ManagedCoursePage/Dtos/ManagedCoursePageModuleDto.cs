using Courses.Application.Modules.Dtos;

namespace Courses.Application.Features.Management.ManagedCoursePage.Dtos;

public record ManagedCoursePageModuleDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required int LessonCount { get; init; }
    public required TimeSpan Duration { get; init; }
    public required ManagedModuleLinks Links { get; init; }
}

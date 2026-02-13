using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Features.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Users.Dtos;

namespace Courses.Application.Features.CoursePage;

public record CoursePageDto
{
    public required CourseDto Course { get; init; }
    public required CourseAnalyticsDto Analytics { get; init; }
    public required CourseStructureDto Structure { get; init; }

    public required IReadOnlyDictionary<Guid, ModuleWithAnalyticsDto> Modules { get; init; }
    public required IReadOnlyDictionary<Guid, LessonDto> Lessons { get; init; }
    public required IReadOnlyDictionary<Guid, UserDto> Instructors { get; init; }
    public required IReadOnlyDictionary<Guid, CategoryDto> Categories { get; init; }
}


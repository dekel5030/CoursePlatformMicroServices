using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Courses.Queries.Dtos;

public record CourseReadDto(
    CourseId Id,
    string Title,
    string Description,
    bool IsPublished,
    string? ImageUrl,
    string? InstructorUserId,
    Money Price,
    DateTimeOffset UpdatedAtUtc,
    IEnumerable<LessonReadDto>? Lessons = null);

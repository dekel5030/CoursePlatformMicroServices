using Domain.Courses.Primitives;
using SharedKernel;

namespace Application.Courses.Queries.Dtos;

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

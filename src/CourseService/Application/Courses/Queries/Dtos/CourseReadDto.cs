using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Courses.Queries.Dtos;

public record CourseReadDto(
    Guid Id,
    string Title,
    string Description,
    Guid? InstructorId,
    decimal Price,
    string Currency,
    int EnrollmentCount,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<string> ImagesUrls,
    IReadOnlyList<LessonReadDto> Lessons);

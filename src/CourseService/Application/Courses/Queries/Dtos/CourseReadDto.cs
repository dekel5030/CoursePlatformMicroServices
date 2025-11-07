using Domain.Courses.Primitives;
using SharedKernel;

namespace Application.Courses.Queries.Dtos;

public record CourseReadDto(
    CourseId Id,
    string Title,
    string Description,
    bool IsPublished,
    bool IsFeatured,
    string? ImageUrl,
    string? InstructorUserId,
    Money Price,
    DateTimeOffset UpdatedAtUtc);

using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Shared.Dtos;

public record InstructorDto(
    Guid Id,
    string FullName,
    string? AvatarUrl
);

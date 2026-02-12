namespace Courses.Application.Users;

public record InstructorDto(
    Guid Id,
    string FullName,
    string? AvatarUrl
);

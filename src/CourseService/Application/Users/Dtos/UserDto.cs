namespace Courses.Application.Users.Dtos;

public sealed record UserDto
{
    public required Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string? AvatarUrl { get; init; }
}

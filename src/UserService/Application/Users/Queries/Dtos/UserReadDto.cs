namespace Users.Application.Users.Queries.Dtos;

public record UserReadDto(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    DateTime? DateOfBirth,
    string? PhoneNumber,
    string? AvatarUrl,
    string? Bio,
    string? LinkedInUrl,
    string? GitHubUrl,
    string? TwitterUrl,
    string? WebsiteUrl,
    bool IsLecturer);
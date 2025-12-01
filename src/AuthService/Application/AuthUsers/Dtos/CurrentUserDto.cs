namespace Application.AuthUsers.Dtos;

public record CurrentUserDto(
    Guid Id,
    string Email,
    string? UserName = null,
    string? FirstName = null,
    string? LastName = null,
    string? AvatarUrl = null
);


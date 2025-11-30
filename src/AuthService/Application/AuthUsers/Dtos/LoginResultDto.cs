namespace Application.AuthUsers.Dtos;

public record LoginResultDto(
    Guid Id, 
    string Email, 
    string? UserName = null, 
    string? FirstName = null,
    string? LastName = null,
    string? AvatarUrl = null
);


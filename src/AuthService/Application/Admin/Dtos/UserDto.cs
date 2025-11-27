namespace Application.Admin.Dtos;

public record UserDto(
    Guid Id,
    string Email,
    bool IsActive,
    IEnumerable<string> Roles,
    IEnumerable<string> DirectPermissions);

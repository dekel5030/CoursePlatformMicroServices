namespace Auth.Application.AuthUsers.Queries;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyList<RoleDto> Roles,
    IReadOnlyList<PermissionDto> Permissions
);

namespace Auth.Application.AuthUsers.Queries.Dtos;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyList<RoleDto> Roles,
    IReadOnlyList<PermissionDto> Permissions
);

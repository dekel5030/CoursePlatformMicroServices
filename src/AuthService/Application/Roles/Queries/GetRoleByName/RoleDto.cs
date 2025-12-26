using Auth.Application.AuthUsers.Queries.Dtos;

namespace Auth.Application.Roles.Queries.GetRoleByName;

public record RoleDto(
    Guid Id,
    string Name,
    IReadOnlyList<PermissionDto> Permissions
);
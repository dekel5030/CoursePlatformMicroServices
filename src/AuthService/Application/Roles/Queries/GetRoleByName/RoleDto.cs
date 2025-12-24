using Auth.Application.AuthUsers.Queries;

namespace Auth.Application.Roles.Queries.GetRoleByName;

public record RoleDto(
    Guid Id,
    string Name,
    IReadOnlyList<PermissionDto> Permissions
);
namespace Auth.Application.Roles.Queries.GetAllRoles;

public record RoleDto(Guid Id, string Name, int PermissionCount, int UserCount);
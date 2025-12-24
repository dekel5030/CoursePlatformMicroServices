using Kernel.Messaging.Abstractions;

namespace Auth.Application.Roles.Queries.GetRoleByName;

public record GetRoleByNameQuery(string RoleName) : IQuery<RoleDto>;

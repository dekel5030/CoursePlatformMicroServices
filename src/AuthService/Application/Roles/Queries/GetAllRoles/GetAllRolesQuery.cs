using Kernel.Messaging.Abstractions;

namespace Auth.Application.Roles.Queries.GetAllRoles;

public record GetAllRolesQuery : IQuery<IReadOnlyCollection<RoleDto>>;

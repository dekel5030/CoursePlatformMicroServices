using Application.Abstractions.Messaging;
using Application.Admin.Dtos;

namespace Application.Admin.Queries.GetRolePermissions;

public record GetRolePermissionsQuery(int RoleId) : IQuery<IEnumerable<PermissionDto>>;

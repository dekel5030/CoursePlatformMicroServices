using Application.Abstractions.Messaging;
using Application.Admin.Dtos;

namespace Application.Admin.Queries.GetUserPermissions;

public record GetUserPermissionsQuery(Guid UserId) : IQuery<IEnumerable<PermissionDto>>;

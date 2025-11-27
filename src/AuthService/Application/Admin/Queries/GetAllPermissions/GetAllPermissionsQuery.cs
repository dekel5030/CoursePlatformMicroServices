using Application.Abstractions.Messaging;
using Application.Admin.Dtos;

namespace Application.Admin.Queries.GetAllPermissions;

public record GetAllPermissionsQuery : IQuery<IEnumerable<PermissionDto>>;

using Application.Abstractions.Messaging;
using Application.Admin.Dtos;

namespace Application.Admin.Queries.GetAllRoles;

public record GetAllRolesQuery : IQuery<IEnumerable<RoleDto>>;

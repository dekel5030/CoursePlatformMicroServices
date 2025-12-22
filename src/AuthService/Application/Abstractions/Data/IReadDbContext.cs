using Auth.Domain.AuthUsers;
using Auth.Domain.Roles;

namespace Auth.Application.Abstractions.Data;

public interface IReadDbContext
{
    IQueryable<AuthUser> Users { get; }
    IQueryable<Role> Roles { get; }
}

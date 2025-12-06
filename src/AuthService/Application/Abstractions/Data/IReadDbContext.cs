using Domain.AuthUsers;
using Domain.Roles;

namespace Application.Abstractions.Data;

public interface IReadDbContext
{
    IQueryable<AuthUser> AuthUsers { get; }
    IQueryable<Role> Roles { get; }
}

using Domain.AuthUsers;
using Domain.Permissions;

namespace Application.Abstractions.Data;

public interface IReadDbContext
{
    IQueryable<AuthUser> AuthUsers { get; }
    IQueryable<Permission> Permissions { get; }
}

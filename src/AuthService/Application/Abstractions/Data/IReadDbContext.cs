using Domain.AuthUsers;
using Domain.Roles;
using Microsoft.AspNetCore.Identity;

namespace Application.Abstractions.Data;

public interface IReadDbContext
{
    IQueryable<AuthUser> AuthUsers { get; }
    IQueryable<Role> Roles { get; }

    IQueryable<IdentityUserClaim<Guid>> UserClaims { get; }
    IQueryable<IdentityRoleClaim<Guid>> RoleClaims { get; }
    IQueryable<IdentityUserRole<Guid>> UserRoles { get; }

    IQueryable<IdentityUserToken<Guid>> UserTokens { get; }
    IQueryable<IdentityUserLogin<Guid>> UserLogins { get; }
}

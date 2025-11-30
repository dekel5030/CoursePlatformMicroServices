using Application.Abstractions.Data;
using Domain.AuthUsers;
using Domain.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class ReadDbContext : IdentityDbContext<AuthUser, Role, Guid>, IReadDbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options)
        : base(options)
    {
    }

    public IQueryable<AuthUser> AuthUsers => base.Users.AsNoTracking();
    public new IQueryable<Role> Roles => base.Roles.AsNoTracking();
    public new IQueryable<IdentityUserClaim<Guid>> UserClaims => base.UserClaims.AsNoTracking();
    public new IQueryable<IdentityRoleClaim<Guid>> RoleClaims => base.RoleClaims.AsNoTracking();
    public new IQueryable<IdentityUserRole<Guid>> UserRoles => base.UserRoles.AsNoTracking();
    public new IQueryable<IdentityUserToken<Guid>> UserTokens => base.UserTokens.AsNoTracking();
    public new IQueryable<IdentityUserLogin<Guid>> UserLogins => base.UserLogins.AsNoTracking();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}

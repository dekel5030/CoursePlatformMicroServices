using Application.Abstractions.Data;
using Domain.AuthUsers;
using Domain.Roles;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class ReadDbContext
    : IdentityDbContext<ApplicationIdentityUser, ApplicationIdentityRole, Guid>, IReadDbContext
{
    public ReadDbContext(DbContextOptions options) : base(options)
    {
    }


    public IQueryable<AuthUser> AuthUsers => base.Users.AsNoTracking().Select(u => u.DomainUser);
    public new IQueryable<Role> Roles => base.Roles.AsNoTracking().Select(r => r.DomainRole);


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);
    }
}

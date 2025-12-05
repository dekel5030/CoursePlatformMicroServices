using Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Identity.Stores;

internal class NoAutoSaveUserStore
    : UserStore<ApplicationIdentityUser, ApplicationIdentityRole, WriteDbContext, Guid>
{
    public NoAutoSaveUserStore(
        WriteDbContext context, 
        IdentityErrorDescriber? describer = null)
        : base(context, describer)
    {
        AutoSaveChanges = false;
    }
}
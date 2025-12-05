using Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Identity.Stores;
internal class NoAutoSaveRoleStore : RoleStore<ApplicationIdentityRole, WriteDbContext, Guid>
{
    public NoAutoSaveRoleStore(
        WriteDbContext context, 
        IdentityErrorDescriber? describer = null)
        : base(context, describer)
    {
        AutoSaveChanges = false;
    }
}

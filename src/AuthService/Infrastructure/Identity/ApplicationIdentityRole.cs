using Domain.Roles;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationIdentityRole : IdentityRole<Guid>
{
    private ApplicationIdentityRole() { }

    public ApplicationIdentityRole(Role domainRole)
    {
        Id = domainRole.Id;
        Name = domainRole.Name;
        NormalizedName = domainRole.Name.ToUpperInvariant();
    }
}

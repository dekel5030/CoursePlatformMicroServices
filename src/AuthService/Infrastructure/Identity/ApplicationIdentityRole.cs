using Domain.Roles;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationIdentityRole : IdentityRole<Guid>
{
    public virtual Role DomainRole { get; set; } = null!;

    public virtual ICollection<IdentityRoleClaim<Guid>> Claims { get; set; }
        = new List<IdentityRoleClaim<Guid>>();

    private ApplicationIdentityRole() { }
    public ApplicationIdentityRole(Role domainRole)
    {
        Name = domainRole.Name;
        Id = domainRole.Id;
        DomainRole = domainRole;
    }
}

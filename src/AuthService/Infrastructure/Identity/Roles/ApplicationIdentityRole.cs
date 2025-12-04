using Domain.Roles;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationIdentityRole : IdentityRole<Guid>
{
    public Role DomainRole { get; set; } = null!;

    private ApplicationIdentityRole() { }
    public ApplicationIdentityRole(Role domainRole)
    {
        Id = domainRole.Id;
        Name = domainRole.Name;
    }
}

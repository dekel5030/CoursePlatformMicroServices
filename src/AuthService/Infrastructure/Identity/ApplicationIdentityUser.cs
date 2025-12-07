using Domain.AuthUsers;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationIdentityUser : IdentityUser<Guid>
{
    public virtual AuthUser DomainUser { get; set; } = null!;
    public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; } = new List<IdentityUserRole<Guid>>();

    private ApplicationIdentityUser() { }

    public ApplicationIdentityUser(AuthUser domainUser)
    {
        Id = domainUser.Id;
        Email = domainUser.Email;
        UserName = domainUser.UserName;
        DomainUser = domainUser;
    }
}

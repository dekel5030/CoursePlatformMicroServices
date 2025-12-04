using Domain.AuthUsers;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationIdentityUser : IdentityUser<Guid>
{
    public AuthUser DomainUser { get; set; } = null!;

    private ApplicationIdentityUser() { }
    public ApplicationIdentityUser(AuthUser domainUser)
    {
        Id = domainUser.Id;
        UserName = domainUser.UserName;
        Email = domainUser.Email;
        DomainUser = domainUser;
    }
}

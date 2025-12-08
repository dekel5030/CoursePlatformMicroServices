using Domain.AuthUsers;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationIdentityUser : IdentityUser<Guid>
{
    private ApplicationIdentityUser()
    {
    }

    public ApplicationIdentityUser(AuthUser user)
    {
    }
}

using Microsoft.AspNetCore.Identity;

namespace Domain.Roles;

public class Role : IdentityRole<Guid>
{
    private Role() { }

    public static Role Create(string roleName)
    {
        return new Role
        {
            Id = Guid.CreateVersion7(),
            Name = roleName,
        };
    }
}
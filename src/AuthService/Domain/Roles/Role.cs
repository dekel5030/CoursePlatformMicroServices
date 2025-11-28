using Microsoft.AspNetCore.Identity;

namespace Domain.Roles;

public class Role : IdentityRole<int>
{
    private Role() { }
    
    public static Role Create(string roleName)
    {
        return new Role
        {
            Id = 0,
            Name = roleName,
        };
    }
}
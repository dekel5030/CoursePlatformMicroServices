using Domain.Permissions;
using Domain.Permissions.Primitives;
using Domain.Roles.Primitives;
using SharedKernel;

namespace Domain.Roles;

public class Role : Entity
{
    private Role() { }

    public RoleId Id { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public static Role Create(string name)
    {
        return new Role
        {
            Id = new RoleId(0), // Will be set by database
            Name = name,
            RolePermissions = new List<RolePermission>()
        };
    }

    public void AddPermission(PermissionId permissionId)
    {
        if (!RolePermissions.Any(rp => rp.PermissionId.Value == permissionId.Value))
        {
            RolePermissions.Add(new RolePermission 
            { 
                RoleId = Id, 
                PermissionId = permissionId 
            });
        }
    }

    public void RemovePermission(PermissionId permissionId)
    {
        var rolePermission = RolePermissions.FirstOrDefault(rp => rp.PermissionId.Value == permissionId.Value);
        if (rolePermission != null)
        {
            RolePermissions.Remove(rolePermission);
        }
    }
}

public class RolePermission
{
    public RoleId RoleId { get; set; } = null!;
    public Role Role { get; set; } = null!;

    public PermissionId PermissionId { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

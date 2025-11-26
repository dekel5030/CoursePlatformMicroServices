using Domain.Permissions.Primitives;
using SharedKernel;

namespace Domain.Permissions;

public class Permission : Entity
{
    private Permission() { }

    public PermissionId Id { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;

    public static Permission Create(string name)
    {
        return new Permission
        {
            Id = new PermissionId(0), // Will be set by database
            Name = name
        };
    }
}

using Domain.Permissions;
using Domain.Permissions.Errors;
using Domain.Roles.Events;
using Kernel;
using SharedKernel;

namespace Domain.Roles;

public class Role : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    private readonly List<Permission> _permissions = new();
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    private Role() { }

    public static Role Create(string roleName)
    {
        var role = new Role
        {
            Id = Guid.CreateVersion7(),
            Name = roleName,
        };

        role.Raise(new RoleCreatedDomainEvent(role));

        return role;
    }

    public Result AddPermission(Permission permission)
    {
        if (_permissions.Any(p => p.Id == permission.Id))
        {
            return Result.Failure(PermissionErrors.PermissionAlreadyAssigned);
        }

        _permissions.Add(permission);
        Raise(new RolePermissionAssignedDomainEvent(this));
        return Result.Success();
    }

    public Result RemovePermission(Permission permission)
    {
        var existingPermission = _permissions.FirstOrDefault(p => p.Id == permission.Id);

        if (existingPermission != null)
        {
            _permissions.Remove(existingPermission);
            Raise(new RolePermissionRemovedDomainEvent(this));
        }

        return Result.Success();
    }
}
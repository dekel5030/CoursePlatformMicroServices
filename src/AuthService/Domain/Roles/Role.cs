using Domain.Permissions;
using Domain.Permissions.Errors;
using Domain.Roles.Errors;
using Domain.Roles.Events;
using Kernel;
using Kernel.Auth.AuthTypes;
using SharedKernel;

namespace Domain.Roles;

public class Role : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    private readonly List<Permission> _permissions = new();
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    private Role() { }

    public static Result<Role> Create(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return Result.Failure<Role>(RoleErrors.NameCannotBeEmpty);
        }

        var role = new Role
        {
            Id = Guid.CreateVersion7(),
            Name = roleName,
        };

        role.Raise(new RoleCreatedDomainEvent(role));

        return Result.Success(role);
    }

    public Result AddPermission(Permission permission)
    {
        if (permission.Effect != EffectType.Allow)
        {
            return Result.Failure(RoleErrors.InvalidPermissionEffect);
        }

        if (_permissions.Contains(permission))
        {
            return Result.Failure(PermissionErrors.PermissionAlreadyAssigned);
        }

        _permissions.Add(permission);
        Raise(new RolePermissionAddedDomainEvent(this, permission));
        return Result.Success();
    }

    public Result RemovePermission(Permission permission)
    {
        if (_permissions.Remove(permission))
        {
            Raise(new RolePermissionRemovedDomainEvent(this, permission));
        }

        return Result.Success();
    }
}
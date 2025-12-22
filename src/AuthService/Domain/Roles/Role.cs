using Auth.Domain.Permissions;
using Auth.Domain.Roles.Errors;
using Auth.Domain.Roles.Events;
using Auth.Domain.Roles.Primitives;
using Kernel;
using Kernel.Auth.AuthTypes;

namespace Auth.Domain.Roles;

public class Role : Entity
{
    public RoleId Id { get; private set; } = null!;
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
            Name = roleName.Trim().ToLowerInvariant(),
            Id = new(Guid.CreateVersion7())
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

        if (_permissions.Any(p => p.Covers(permission)))
        {
            return Result.Success();
        }

        _permissions.RemoveAll(p => permission.Covers(p));
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
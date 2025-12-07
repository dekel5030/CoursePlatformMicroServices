using Domain.AuthUsers.Errors;
using Domain.AuthUsers.Events;
using Domain.Permissions;
using Domain.Roles;
using Kernel;
using SharedKernel;

namespace Domain.AuthUsers;

public class AuthUser : Entity
{
    private readonly List<Role> _roles = new();
    private readonly List<UserPermission> _permissions = new();

    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string UserName { get; private set; } = null!;
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
    public IReadOnlyCollection<UserPermission> Permissions => _permissions.AsReadOnly();

    private AuthUser() { }

    public static AuthUser Create(string email, string userName, Role initialRole)
    {
        var authUser = new AuthUser
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            UserName = string.IsNullOrEmpty(userName) ? email : userName
        };

        authUser._roles.Add(initialRole);

        authUser.Raise(new UserRegisteredDomainEvent(
            authUser.Id,
            email,
            DateTime.UtcNow));

        return authUser;
    }

    public Result AddRole(Role role)
    {
        if (_roles.Any(r => r.Id == role.Id))
        {
            return Result.Failure(AuthUserErrors.RoleAlreadyExists);
        }

        _roles.Add(role);
        Raise(new UserRoleAddedDomainEvent(this, role));

        return Result.Success();
    }

    public Result RemoveRole(Role role)
    {
        var roleToRemove = _roles.FirstOrDefault(r => r.Id == role.Id);

        if (roleToRemove is not null)
        {
            _roles.Remove(roleToRemove);
            Raise(new UserRoleRemovedDomainEvent(this, roleToRemove));
        }

        return Result.Success();
    }

    public Result AddPermission(UserPermission permission)
    {
        if (_permissions.Contains(permission))
        {
            return Result.Failure(AuthUserErrors.PermissionAlreadyExists);
        }

        _permissions.Add(permission);
        Raise(new UserPermissionAddedDomainEvent(this, permission));

        return Result.Success();
    }

    public Result RemovePermission(UserPermission permission)
    {
        if (_permissions.Remove(permission))
        {
            Raise(new UserPermissionRemovedDomainEvent(this, permission));
        }

        return Result.Success();
    }
}

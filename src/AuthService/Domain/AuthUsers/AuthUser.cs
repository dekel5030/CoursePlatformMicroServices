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
    private readonly List<Permission> _permissions = new();

    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string UserName { get; private set; } = null!;
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    private AuthUser() { }

    public static Result<AuthUser> Create(string email, string userName, Role initialRole)
    {
        var authUser = new AuthUser
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            UserName = string.IsNullOrEmpty(userName) ? email : userName
        };

        authUser._roles.Add(initialRole);

        authUser.Raise(new UserRegisteredDomainEvent(authUser));
        authUser.Raise(new UserRoleAddedDomainEvent(authUser, initialRole));

        return Result.Success(authUser);
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

    public Result AddPermission(Permission permission)
    {
        if (_permissions.Contains(permission))
        {
            return Result.Failure(AuthUserErrors.PermissionAlreadyExists);
        }

        _permissions.Add(permission);
        Raise(new UserPermissionAddedDomainEvent(this, permission));

        return Result.Success();
    }

    public Result RemovePermission(Permission permission)
    {
        if (_permissions.Remove(permission))
        {
            Raise(new UserPermissionRemovedDomainEvent(this, permission));
        }

        return Result.Success();
    }

    public Result AddPermissions(IEnumerable<Permission> permissions)
    {
        IEnumerable<Error> errors = Enumerable.Empty<Error>();

        foreach (var permission in permissions)
        {
            if (_permissions.Contains(permission))
            {
                errors = errors.Append(AuthUserErrors.PermissionAlreadyExistsWithValue(permission.ToString()));
            }
        }

        if (errors.Any())
        {
            return Result.Failure(new ValidationError(errors));
        }

        foreach (var permission in permissions)
        {
            _permissions.Add(permission);
            Raise(new UserPermissionAddedDomainEvent(this, permission));
        }

        return Result.Success();
    }

    public Result RemovePermissions(IEnumerable<Permission> permissions)
    {
        foreach (var permission in permissions)
        {
            Result result = RemovePermission(permission);
            if (result.IsFailure)
            {
                return result;
            }
        }

        return Result.Success();
    }
}
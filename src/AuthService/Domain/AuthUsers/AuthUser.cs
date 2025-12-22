using Domain.AuthUsers.Errors;
using Domain.AuthUsers.Events;
using Domain.AuthUsers.Primitives;
using Domain.Permissions;
using Domain.Roles;
using Kernel;

namespace Domain.AuthUsers;

public class AuthUser : Entity
{
    private readonly List<Role> _roles = new();
    private readonly List<Permission> _permissions = new();

    public AuthUserId Id { get; private set; }
    public IdentityProviderId IdentityId { get; private set; }
    public FullName FullName { get; private set; }
    public Email Email { get; private set; }
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    #pragma warning disable CS8618
    private AuthUser() { }
    #pragma warning restore CS8618

    private AuthUser(
        AuthUserId id,
        IdentityProviderId identityProviderId,
        FullName fullName,
        Email email) 
    {
        Id = id;
        IdentityId = identityProviderId;
        FullName = fullName;
        Email = email;
    }

    public static Result<AuthUser> Create(
        IdentityProviderId identityId,
        FullName fullName,
        Email email,
        Role initialRole)
    {
        var authUser = new AuthUser(
            new AuthUserId(Guid.CreateVersion7()),
            identityId,
            fullName,
            email);

        authUser.Raise(new UserRegisteredDomainEvent(authUser));
        authUser.AddRole(initialRole);

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
        if (_permissions.Any(p => p.Covers(permission)))
        {
            return Result.Success();
        }

        _permissions.RemoveAll(p => permission.Covers(p));
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
}
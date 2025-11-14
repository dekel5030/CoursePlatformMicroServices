using Domain.AuthUsers.Events;
using Domain.AuthUsers.Primitives;
using Domain.Permissions.Primitives;
using Domain.Roles;
using Domain.Roles.Primitives;
using SharedKernel;

namespace Domain.AuthUsers;

public class AuthUser : Entity
{
    private AuthUser() { }

    public AuthUserId Id { get; private set; } = null!;
    public string? UserId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<UserPermission> UserPermissions { get; private set; } = new List<UserPermission>();

    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsConfirmed { get; private set; } = false;
    public int FailedLoginAttempts { get; private set; } = 0;
    public DateTime? LockedUntil { get; private set; } = null;

    public static AuthUser Create(
        string email,
        string passwordHash,
        RoleId defaultRoleId)
    {
        var authUser = new AuthUser
        {
            Id = new AuthUserId(Guid.CreateVersion7()),
            UserId = null, // Will be set when UserService responds
            Email = email,
            PasswordHash = passwordHash,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            IsConfirmed = false,
            FailedLoginAttempts = 0,
            UserRoles = new List<UserRole>(),
            UserPermissions = new List<UserPermission>()
        };

        // Assign default role
        authUser.AddRole(defaultRoleId);

        // Raise domain event
        authUser.Raise(new UserRegisteredDomainEvent(
            authUser.Id,
            email,
            DateTime.UtcNow));

        return authUser;
    }

    public void Login()
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;

        Raise(new UserLoggedInDomainEvent(
            Id,
            Email,
            DateTime.UtcNow));
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= 5)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(30);
        }
    }

    public bool IsLocked()
    {
        return LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;
    }

    public void AddRole(RoleId roleId)
    {
        if (!UserRoles.Any(ur => ur.RoleId.Value == roleId.Value))
        {
            UserRoles.Add(new UserRole 
            { 
                UserId = Id, 
                RoleId = roleId 
            });
        }
    }

    public void AssignRole(RoleId roleId, string roleName)
    {
        AddRole(roleId);

        Raise(new RoleAssignedDomainEvent(
            Id,
            roleId,
            roleName,
            DateTime.UtcNow));
    }

    public void RemoveRole(RoleId roleId)
    {
        var userRole = UserRoles.FirstOrDefault(ur => ur.RoleId.Value == roleId.Value);
        if (userRole != null)
        {
            UserRoles.Remove(userRole);
        }
    }

    public void AddPermission(PermissionId permissionId)
    {
        if (!UserPermissions.Any(up => up.PermissionId.Value == permissionId.Value))
        {
            UserPermissions.Add(new UserPermission 
            { 
                UserId = Id, 
                PermissionId = permissionId 
            });
        }
    }

    public void RemovePermission(PermissionId permissionId)
    {
        var userPermission = UserPermissions.FirstOrDefault(up => up.PermissionId.Value == permissionId.Value);
        if (userPermission != null)
        {
            UserPermissions.Remove(userPermission);
        }
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Confirm()
    {
        IsConfirmed = true;
    }

    public void LinkUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("UserId cannot be null or empty", nameof(userId));
        }

        if (!string.IsNullOrWhiteSpace(UserId))
        {
            throw new InvalidOperationException("UserId has already been linked");
        }

        UserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}

public class UserRole
{
    public AuthUserId UserId { get; set; } = null!;
    public AuthUser User { get; set; } = null!;

    public RoleId RoleId { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

public class UserPermission
{
    public AuthUserId UserId { get; set; } = null!;
    public AuthUser User { get; set; } = null!;

    public PermissionId PermissionId { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

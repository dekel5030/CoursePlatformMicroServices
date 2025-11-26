using Domain.AuthUsers.Events;
using Domain.AuthUsers.Primitives;
using Domain.Permissions;
using Domain.Permissions.Primitives;
using Domain.Roles;
using Domain.Roles.Primitives;
using SharedKernel;

namespace Domain.AuthUsers;

public class AuthUser : Entity
{
    private AuthUser() { }

    public AuthUserId Id { get; private set; } = null!;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<UserPermission> UserPermissions { get; private set; } = new List<UserPermission>();

    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsConfirmed { get; private set; } = false;
    public int FailedLoginAttempts { get; private set; } = 0;
    public DateTime? LockedUntil { get; private set; } = null;
    
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }

    public static AuthUser Create(
        string email,
        string passwordHash,
        RoleId defaultRoleId)
    {
        var authUser = new AuthUser
        {
            Id = new AuthUserId(Guid.CreateVersion7()),
            Email = email,
            PasswordHash = passwordHash,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            IsConfirmed = false,
            FailedLoginAttempts = 0,
            UserRoles = new List<UserRole>(),
            UserPermissions = new List<UserPermission>()
        };

        authUser.AddRole(defaultRoleId);

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

    public void SetRefreshToken(string refreshTokenHash, DateTime expiresAt)
    {
        RefreshToken = refreshTokenHash;
        RefreshTokenExpiresAt = expiresAt;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresAt = null;
    }

    public bool IsRefreshTokenValid(string refreshTokenHash)
    {
        if (string.IsNullOrEmpty(RefreshToken) || !RefreshTokenExpiresAt.HasValue)
        {
            return false;
        }

        return RefreshToken == refreshTokenHash && RefreshTokenExpiresAt.Value > DateTime.UtcNow;
    }

    public IEnumerable<Permission> GetPermissions()
    {
        var rolePermissions = UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission);
        var directPermissions = UserPermissions
            .Select(up => up.Permission);
        return rolePermissions
            .Concat(directPermissions)
            .Distinct();
    }

    public IEnumerable<Role> GetRoles()
    {
        return UserRoles.Select(userRole => userRole.Role);
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

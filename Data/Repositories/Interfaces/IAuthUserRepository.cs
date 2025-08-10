using AuthService.Dtos.AuthUsers;
using AuthService.Models;

namespace AuthService.Data.Repositories.Interfaces;

public interface IAuthUserRepository
{
    Task<AuthUser?> GetUserByIdAsync(int userId);
    Task<AuthUser?> GetUserByEmailAsync(string email);
    Task<(IEnumerable<AuthUser> users, int totalCount)> SearchUsersAsync(UserSearch query);

    Task<AuthUser?> GetUserWithAccessDataByIdAsync(int userId);
    Task<AuthUser?> GetUserWithAccessDataByEmailAsync(string email);

    Task AddUserAsync(AuthUser authUser);
    Task RemoveUserAsync(AuthUser authUser);

    Task<(IEnumerable<Permission> permissions, int totalCount)> GetUserPermissionsAsync(int userId);
    Task<(IEnumerable<Role> roles, int totalCount)> GetUserRolesAsync(int userId);

    Task AssignRoleAsync(UserRole userRole);
    Task UnassignRoleAsync(UserRole userRole);

    Task AddPermissionAsync(UserPermission userPermission);
    Task RemovePermissionAsync(UserPermission userPermission);

    Task<bool> ExistsByIdAsync(int userId);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> HasPermissionAsync(int userId, int permissionId);
    Task<bool> HasRoleAsync(int userId, int roleId);
}
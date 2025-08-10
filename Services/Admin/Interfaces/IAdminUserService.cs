using AuthService.Dtos;
using AuthService.Dtos.AuthUsers;
using AuthService.Dtos.Permissions;
using AuthService.Dtos.Roles;
using Common;
using AuthUserReadDto = AuthService.Dtos.AuthUsers.AuthUserReadDto;

namespace AuthService.Services.Admin.Interfaces;

public interface IAdminUserService
{
    Task<Result<AuthUserReadDto>> GetUserByIdAsync(int userId);
    Task<PagedResponseDto<AuthUserReadDto>> SearchUsersAsync(UserSearch query);
    Task<Result<bool>> RemoveUserAsync(int userId);

    Task<PagedResponseDto<RoleReadDto>> GetUserRolesAsync(int userId);
    Task<Result<bool>> AssignRoleAsync(int userId, int roleId);
    Task<Result<bool>> UnassignRoleAsync(int userId, int roleId);

    Task<PagedResponseDto<PermissionReadDto>> GetUserPermissionsAsync(int userId);
    Task<Result<bool>> AddPermissionAsync(int userId, int permissionId);
    Task<Result<bool>> RemovePermissionAsync(int userId, int permissionId);
}
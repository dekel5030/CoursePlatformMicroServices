using AuthService.Dtos;
using AuthService.Dtos.AuthUsers;
using AuthService.Dtos.Permissions;
using AuthService.Dtos.Roles;
using Common;
using UserReadDto = AuthService.Dtos.AuthUsers.UserReadDto;

namespace AuthService.Services.Admin.Interfaces;

public interface IAdminUserService
{
    Task<Result<UserReadDto>> GetUserByIdAsync(int userId);
    Task<PagedResponseDto<UserReadDto>> SearchUsersAsync(UserSearchDto query);
    Task<Result<bool>> RemoveUserAsync(int userId);

    Task<PagedResponseDto<RoleReadDto>> GetUserRolesAsync(int userId);
    Task<Result<bool>> AddRoleAsync(int userId, UserAssignRoleDto assignRoleDto);
    Task<Result<bool>> RemoveRoleAsync(int userId, int roleId);

    Task<PagedResponseDto<PermissionReadDto>> GetUserPermissionsAsync(int userId);
    Task<Result<bool>> AddPermissionAsync(int userId, UserAssignPermissionDto assignPermissionDto);
    Task<Result<bool>> RemovePermissionAsync(int userId, int permissionId);
}
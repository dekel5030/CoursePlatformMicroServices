using AuthService.Dtos;
using AuthService.Dtos.Permissions;
using AuthService.Dtos.Roles;
using Common;

namespace AuthService.Services.Admin.Interfaces;

public interface IAdminRoleService
{
    Task<Result<RoleReadDto>> GetRoleByIdAsync(int id);
    Task<PagedResponseDto<RoleReadDto>> GetAllRolesAsync();
    Task<Result<RoleReadDto>> CreateRoleAsync(RoleCreateDto createDto);
    Task<Result<bool>> DeleteRoleByIdAsync(int id);
    
    Task<PagedResponseDto<PermissionReadDto>> GetRolePermissionsAsync(int id);
    Task<Result<bool>> AssignPermissionsAsync(int id, RoleAssignPermissionsDto permissions);
    Task<Result<bool>> RemovePermissionAsync(int roleId, int permissionId);
}
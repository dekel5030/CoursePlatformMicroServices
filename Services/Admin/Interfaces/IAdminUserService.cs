namespace AuthService.Services.Admin.Interfaces;

public interface IAdminUserService
{
    Task AddUserPermissionAsync(int id, UserAssignPermissionDto assignPermissionDto);
    Task AddUserRoleAsync(int id, UserAssignRoleDto assignRoleDto);
    Task CreateUserAsync(CreateUserDto createUserDto);
    Task DeleteUserAsync(int id);
    Task GetUserByIdAsync(int id);
    Task GetUserPermissionsAsync(int id);
    Task GetUserRolesAsync(int id);
    Task RemoveUserPermissionAsync(int id, int permissionId);
    Task RemoveUserRoleAsync(int id, int roleId);
    Task SearchUsersAsync(UserSearchDto query);
}
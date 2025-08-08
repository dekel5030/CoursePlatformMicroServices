
using AuthService.Data.Repositories.Interfaces;

namespace AuthService.Services;

public class PermissionService : IPermissionService
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IUserPermissionRepository _userPermissionRepository;

    public PermissionService(
        IRolePermissionRepository rolePermissionRepository,
        IUserPermissionRepository userPermissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _userPermissionRepository = userPermissionRepository;
    }

    public async Task<HashSet<string>> GetPermissionAsync(int userId)
    {
        var userPermissions = await _userPermissionRepository.GetPermissionsAsync(userId);
        var rolePermissions = await _rolePermissionRepository.GetPermissionsAsync(userId);

        return new HashSet<string>(
            userPermissions.Select(p => p.Name)
                .Union(rolePermissions.Select(p => p.Name))
        );
    }
}
using AuthService.Data.Repositories.Interfaces;
using AuthService.Dtos;
using AuthService.Dtos.Permissions;
using AuthService.Dtos.Roles;
using AuthService.Models;
using AutoMapper;
using Common;
using Common.Errors;

namespace AuthService.Services.Admin.Interfaces;

public class AdminRoleService : IAdminRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;

    public AdminRoleService(
        IRoleRepository roleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IPermissionRepository permissionRepository,
        IMapper mapper)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    public async Task<Result<RoleReadDto>> GetRoleByIdAsync(int id)
    {
        var role = await _roleRepository.GetRoleByIdAsync(id);

        if (role == null)
        {
            return Result<RoleReadDto>.Failure(AuthErrors.RoleNotFound);
        }

        var roleDto = _mapper.Map<RoleReadDto>(role);

        return Result<RoleReadDto>.Success(roleDto);
    }

    public async Task<PagedResponseDto<RoleReadDto>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        var rolesCount = roles.Count();

        return new PagedResponseDto<RoleReadDto>
        {
            Items = _mapper.Map<IEnumerable<RoleReadDto>>(roles),
            TotalCount = rolesCount,
            PageNumber = 1,
            PageSize = rolesCount
        };
    }

    public async Task<Result<RoleReadDto>> CreateRoleAsync(RoleCreateDto createDto)
    {
        var roleExists = await _roleRepository.GetRoleByNameAsync(createDto.Name);

        if (roleExists != null)
        {
            return Result<RoleReadDto>.Failure(AuthErrors.RoleAlreadyExists);
        }

        var role = _mapper.Map<Role>(createDto);

        await _roleRepository.AddAsync(role);
        await _roleRepository.SaveChangesAsync();
        
        var roleDto = _mapper.Map<RoleReadDto>(role);
        
        return Result<RoleReadDto>.Success(roleDto);
    }

    public async Task<Result<bool>> DeleteRoleByIdAsync(int id)
    {
        var role = await _roleRepository.GetRoleByIdAsync(id);

        if (role == null)
        {
            return Result<bool>.Failure(AuthErrors.RoleNotFound);
        }

        _roleRepository.Remove(role);

        await _roleRepository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<PagedResponseDto<PermissionReadDto>> GetRolePermissionsAsync(int id)
    {
        var permissions = await _rolePermissionRepository.GetPermissionsAsync(id);
        var permissionsCount = permissions.Count();

        return new PagedResponseDto<PermissionReadDto>
        {
            Items = _mapper.Map<IEnumerable<PermissionReadDto>>(permissions),
            TotalCount = permissionsCount,
            PageNumber = 1,
            PageSize = permissionsCount
        };
    }

    public async Task<Result<bool>> AssignPermissionsAsync(int roleId, RoleAssignPermissionsDto permissionsDto)
    {
        var existing = await _rolePermissionRepository.GetPermissionsAsync(roleId);
        var existingPermissionNames = existing.Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newPermissions = permissionsDto.Permissions
            .Where(p => !existingPermissionNames.Contains(p))
            .ToList();
        var newPermissionIds = new List<int>();

        foreach (var name in newPermissions)
        {
            var permission = await _permissionRepository.GetPermissionByNameAsync(name);
            if (permission is null)
            {
                return Result<bool>.Failure(AuthErrors.OneOrMorePermissionsNotFound);
            }

            newPermissionIds.Add(permission.Id);
        }
        
        await _rolePermissionRepository.AssignPermissionsAsync(roleId, newPermissionIds);
        await _rolePermissionRepository.SaveChangesAsync();
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemovePermissionAsync(int roleId, int permissionId)
    {
        var permission = await _permissionRepository.GetPermissionByIdAsync(permissionId);

        if (permission is null)
        {
            return Result<bool>.Failure(AuthErrors.PermissionNotFound);
        }

        await _rolePermissionRepository.RemovePermissionAsync(roleId, permissionId);
        await _rolePermissionRepository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}

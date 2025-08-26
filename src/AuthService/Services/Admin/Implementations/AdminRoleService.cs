using AuthService.Data;
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
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AdminRoleService(
        UnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<RoleReadDto>> GetRoleByIdAsync(int id)
    {
        var role = await _unitOfWork.RoleRepository.GetRoleByIdAsync(id);

        if (role == null)
        {
            return Result<RoleReadDto>.Failure(AuthErrors.RoleNotFound);
        }

        var roleDto = _mapper.Map<RoleReadDto>(role);

        return Result<RoleReadDto>.Success(roleDto);
    }

    public async Task<PagedResponseDto<RoleReadDto>> GetAllRolesAsync()
    {
        var roles = await _unitOfWork.RoleRepository.GetAllAsync();
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
        var roleExists = await _unitOfWork.RoleRepository.ExistsByNameAsync(createDto.Name);

        if (roleExists)
        {
            return Result<RoleReadDto>.Failure(AuthErrors.RoleAlreadyExists);
        }

        var role = _mapper.Map<Role>(createDto);

        await _unitOfWork.RoleRepository.AddRoleAsync(role);
        await _unitOfWork.SaveChangesAsync();

        var roleDto = _mapper.Map<RoleReadDto>(role);
        
        return Result<RoleReadDto>.Success(roleDto);
    }

    public async Task<Result<bool>> DeleteRoleByIdAsync(int id)
    {
        var role = await _unitOfWork.RoleRepository.GetRoleByIdAsync(id);

        if (role == null)
        {
            return Result<bool>.Failure(AuthErrors.RoleNotFound);
        }

        await _unitOfWork.RoleRepository.RemoveRoleAsync(role);

        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<PagedResponseDto<PermissionReadDto>> GetRolePermissionsAsync(int id)
    {
        var (permissions, TotalCount) = await _unitOfWork.RoleRepository.GetRolePermissionsAsync(id);

        return new PagedResponseDto<PermissionReadDto>
        {
            Items = _mapper.Map<IEnumerable<PermissionReadDto>>(permissions),
            TotalCount = TotalCount,
            PageNumber = 1,
            PageSize = TotalCount
        };
    }

    public async Task<Result<bool>> AssignPermissionsAsync(int roleId, RoleAssignPermissionsDto permissionsDto)
    {
        var (existingPermissions, TotalCount) = await _unitOfWork.RoleRepository.GetRolePermissionsAsync(roleId);

        var existingPermissionNames = existingPermissions.Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newPermissions = permissionsDto.Permissions
            .Where(p => !existingPermissionNames.Contains(p))
            .ToList();

        var rolePermissions = new List<RolePermission>();

        foreach (var name in newPermissions)
        {
            var permission = await _unitOfWork.PermissionRepository.GetPermissionByNameAsync(name);

            if (permission is null)
            {
                return Result<bool>.Failure(AuthErrors.OneOrMorePermissionsNotFound);
            }

            rolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permission.Id
            });
        }

        await _unitOfWork.RoleRepository.AddPermissionsAsync(rolePermissions.ToArray());
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemovePermissionAsync(int roleId, int permissionId)
    {
        var roleHasPermission = await _unitOfWork.RoleRepository.HasPermission(roleId, permissionId);

        if (!roleHasPermission)
        {
            return Result<bool>.Failure(AuthErrors.PermissionNotFound);
        }

        var rolePermission = new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        await _unitOfWork.RoleRepository.RemovePermissionAsync(rolePermission);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}

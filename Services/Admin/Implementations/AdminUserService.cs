using AuthService.Data;
using AuthService.Dtos;
using AuthService.Dtos.AuthUsers;
using AuthService.Dtos.Permissions;
using AuthService.Dtos.Roles;
using AuthService.Models;
using AuthService.Services.Admin.Interfaces;
using AutoMapper;
using Common;
using Common.Errors;
using AuthUserReadDto = AuthService.Dtos.AuthUsers.AuthUserReadDto;

namespace AuthService.Services.Admin.Implementations;

public class AdminUserService : IAdminUserService
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AdminUserService(
        UnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AuthUserReadDto>> GetUserByIdAsync(int userId)
    {
        var user = await _unitOfWork.AuthUserRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return Result<AuthUserReadDto>.Failure(AuthErrors.UserNotFound);
        }

        var userReadDto = _mapper.Map<AuthUserReadDto>(user);

        return Result<AuthUserReadDto>.Success(userReadDto);
    }

    public async Task<PagedResponseDto<AuthUserReadDto>> SearchUsersAsync(UserSearchDto query)
    {
        var (users, totalCount) = await _unitOfWork.AuthUserRepository.SearchUsersAsync(query);

        return new PagedResponseDto<AuthUserReadDto>
        {
            Items = _mapper.Map<List<AuthUserReadDto>>(users),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    public async Task<Result<bool>> RemoveUserAsync(int userId)
    {
        var user = await _unitOfWork.AuthUserRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        await _unitOfWork.AuthUserRepository.RemoveUserAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<PagedResponseDto<PermissionReadDto>> GetUserPermissionsAsync(int userId)
    {
        var (permissions, totalCount) = await _unitOfWork.AuthUserRepository.GetUserPermissionsAsync(userId);

        return new PagedResponseDto<PermissionReadDto>
        {
            Items = _mapper.Map<List<PermissionReadDto>>(permissions),
            TotalCount = totalCount,
            PageNumber = 1,
            PageSize = totalCount
        };
    }

    public async Task<Result<bool>> AddPermissionAsync(int userId, int permissionId)
    {
        var userExists = await _unitOfWork.AuthUserRepository.ExistsByIdAsync(userId);

        if (!userExists)
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        var permissionExists = await _unitOfWork.PermissionRepository.ExistsByIdAsync(permissionId);

        if (!permissionExists)
        {
            return Result<bool>.Failure(AuthErrors.PermissionNotFound);
        }

        var userPermission = new UserPermission
        {
            UserId = userId,
            PermissionId = permissionId
        };

        await _unitOfWork.AuthUserRepository.AddPermissionAsync(userPermission);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemovePermissionAsync(int userId, int permissionId)
    {
        var userExists = await _unitOfWork.AuthUserRepository.ExistsByIdAsync(userId);

        if (!userExists)
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        var permissionExists = await _unitOfWork.AuthUserRepository.HasPermissionAsync(userId, permissionId);

        if (!permissionExists)
        {
            return Result<bool>.Failure(AuthErrors.PermissionNotFound);
        }

        var userPermission = new UserPermission
        {
            UserId = userId,
            PermissionId = permissionId
        };

        await _unitOfWork.AuthUserRepository.RemovePermissionAsync(userPermission);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }


    public async Task<PagedResponseDto<RoleReadDto>> GetUserRolesAsync(int userId)
    {
        var (roles, totalCount) = await _unitOfWork.AuthUserRepository.GetUserRolesAsync(userId);

        return new PagedResponseDto<RoleReadDto>
        {
            Items = _mapper.Map<List<RoleReadDto>>(roles),
            TotalCount = totalCount,
            PageNumber = 1,
            PageSize = totalCount
        };
    }

    public async Task<Result<bool>> AssignRoleAsync(int userId, int roleId)
    {
        var userExists = await _unitOfWork.AuthUserRepository.ExistsByIdAsync(userId);

        if (!userExists)
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        var roleExists = await _unitOfWork.RoleRepository.ExistsByIdAsync(roleId);

        if (!roleExists)
        {
            return Result<bool>.Failure(AuthErrors.RoleNotFound);
        }

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        await _unitOfWork.AuthUserRepository.AssignRoleAsync(userRole);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UnassignRoleAsync(int userId, int roleId)
    {
        var userExists = await _unitOfWork.AuthUserRepository.ExistsByIdAsync(userId);

        if (!userExists)
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        var roleExists = await _unitOfWork.RoleRepository.ExistsByIdAsync(roleId);

        if (!roleExists)
        {
            return Result<bool>.Failure(AuthErrors.RoleNotFound);
        }

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        await _unitOfWork.AuthUserRepository.UnassignRoleAsync(userRole);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}

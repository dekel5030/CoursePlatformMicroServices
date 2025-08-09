using AuthService.Data.Repositories.Interfaces;
using AuthService.Dtos;
using AuthService.Dtos.AuthUsers;
using AuthService.Dtos.Permissions;
using AuthService.Dtos.Roles;
using AuthService.Services.Admin.Interfaces;
using AutoMapper;
using Common;
using Common.Errors;
using UserReadDto = AuthService.Dtos.AuthUsers.UserReadDto;

namespace AuthService.Services.Admin.Implementations;

public class AdminUserService : IAdminUserService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IAuthUserRepository _userRepository;

    public AdminUserService(
        IPermissionRepository permissionRepository,
        IAuthUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IUserPermissionRepository userPermissionRepository,
        IRoleRepository roleRepository,
        IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _userPermissionRepository = userPermissionRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserReadDto>> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetAuthUserByIdAsync(userId);

        if (user == null)
        {
            return Result<UserReadDto>.Failure(AuthErrors.UserNotFound);
        }

        var userReadDto = _mapper.Map<UserReadDto>(user);

        return Result<UserReadDto>.Success(userReadDto);
    }

    public async Task<PagedResponseDto<UserReadDto>> SearchUsersAsync(UserSearchDto query)
    {
        var (users, totalCount) = await _userRepository.SearchUsersAsync(query);

        return new PagedResponseDto<UserReadDto>
        {
            Items = _mapper.Map<List<UserReadDto>>(users),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    public async Task<Result<bool>> RemoveUserAsync(int userId)
    {
        var user = await _userRepository.ExistsByIdAsync(userId);

        if (!user)
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        await _userRepository.RemoveUserAsync(userId);
        await _userRepository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<PagedResponseDto<PermissionReadDto>> GetUserPermissionsAsync(int userId)
    {
        var (permissions, totalCount) = await _userPermissionRepository.GetUserPermissionsAsync(userId);

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
        var userExists = await _userRepository.ExistsByIdAsync(userId);

        if (!userExists)
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        var permissionExists = await _permissionRepository.ExistsByIdAsync(permissionId);

        if (!permissionExists)
        {
            return Result<bool>.Failure(AuthErrors.PermissionNotFound);
        }

        await _userPermissionRepository.AddPermissionAsync(userId, permissionId);
        await _userPermissionRepository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemovePermissionAsync(int userId, int permissionId)
    {
        var userExists = await _userRepository.ExistsByIdAsync(userId);

        if (!userExists)
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        var permissionExists = await _userPermissionRepository.ExistsByIdAsync(userId, permissionId);

        if (!permissionExists)
        {
            return Result<bool>.Failure(AuthErrors.PermissionNotFound);
        }

        await _userPermissionRepository.RemovePermissionAsync(userId, permissionId);
        await _userPermissionRepository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }


    public async Task<PagedResponseDto<RoleReadDto>> GetUserRolesAsync(int userId)
    {
        var (roles, totalCount) = await _userRoleRepository.GetUserRolesAsync(userId);

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
        var userExists = await _userRepository.ExistsByIdAsync(userId);

        if (!userExists)
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        var roleExists = await _roleRepository.ExistsByIdAsync(roleId);

        if (!roleExists)
        {
            return Result<bool>.Failure(AuthErrors.RoleNotFound);
        }

        await _userRoleRepository.AssignRoleAsync(userId, roleId);
        await _userRoleRepository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UnAssignRoleAsync(int userId, int roleId)
    {
        var userExists = await _userRepository.ExistsByIdAsync(userId);

        if (!userExists)
        {
            return Result<bool>.Failure(AuthErrors.UserNotFound);
        }

        var roleExists = await _roleRepository.ExistsByIdAsync(roleId);

        if (!roleExists)
        {
            return Result<bool>.Failure(AuthErrors.RoleNotFound);
        }

        await _userRoleRepository.UnAssignRoleAsync(userId, roleId);
        await _userRoleRepository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}

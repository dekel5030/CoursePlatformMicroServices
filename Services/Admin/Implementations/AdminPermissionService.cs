using AuthService.Data.Repositories.Interfaces;
using AuthService.Dtos;
using AuthService.Dtos.Permissions;
using AuthService.Models;
using AuthService.Services.Admin.Interfaces;
using AutoMapper;
using Common;
using Common.Errors;

namespace AuthService.Services.Admin.Implementations;

public class AdminPermissionService : IAdminPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;

    public AdminPermissionService(IPermissionRepository permissionRepository, IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    public async Task<Result<PermissionReadDto>> CreatePermissionAsync(PermissionCreateDto createDto)
    {
        var permissionExists = await _permissionRepository.GetPermissionByNameAsync(createDto.Name);

        if (permissionExists != null)
        {
            return Result<PermissionReadDto>.Failure(AuthErrors.PermissionAlreadyExists);
        }

        var permission = _mapper.Map<Permission>(createDto);

        await _permissionRepository.AddPermissionAsync(permission);
        await _permissionRepository.SaveChangesAsync();

        return Result<PermissionReadDto>.Success(_mapper.Map<PermissionReadDto>(permission));
    }

    public async Task<Result<bool>> DeletePermissionAsync(int id)
    {
        var permission = await _permissionRepository.GetPermissionByIdAsync(id);

        if (permission == null)
        {
            return Result<bool>.Failure(AuthErrors.PermissionNotFound);
        }

        _permissionRepository.DeletePermission(permission);

        await _permissionRepository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResponseDto<PermissionReadDto>>> SearchPermissionsAsync(PermissionSearchDto query)
    {
        var permissions = await _permissionRepository.GetPermissionsAsync(query);
        var permissionDtos = _mapper.Map<IEnumerable<PermissionReadDto>>(permissions);

        return Result<PagedResponseDto<PermissionReadDto>>.Success(
            new PagedResponseDto<PermissionReadDto>
            {
                Items = permissionDtos,
                TotalCount = permissionDtos.Count(),
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }
        );
    }

    public async Task<Result<PermissionReadDto>> GetPermissionByIdAsync(int id)
    {
        var permission = await _permissionRepository.GetPermissionByIdAsync(id);

        if (permission == null)
        {
            return Result<PermissionReadDto>.Failure(AuthErrors.PermissionNotFound);
        }

        return Result<PermissionReadDto>.Success(_mapper.Map<PermissionReadDto>(permission));
    }
}
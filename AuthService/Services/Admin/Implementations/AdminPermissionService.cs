using AuthService.Data;
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
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AdminPermissionService(UnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PermissionReadDto>> GetPermissionByIdAsync(int id)
    {
        var permission = await _unitOfWork.PermissionRepository.GetPermissionByIdAsync(id);

        if (permission == null)
        {
            return Result<PermissionReadDto>.Failure(AuthErrors.PermissionNotFound);
        }

        return Result<PermissionReadDto>.Success(_mapper.Map<PermissionReadDto>(permission));
    }

    public async Task<Result<PagedResponseDto<PermissionReadDto>>> SearchPermissionsAsync(PermissionSearchDto query)
    {
        var (permissions, totalCount) = await _unitOfWork.PermissionRepository.GetAllPermissionsAsync();
        var permissionDtos = _mapper.Map<IEnumerable<PermissionReadDto>>(permissions);

        return Result<PagedResponseDto<PermissionReadDto>>.Success(
            new PagedResponseDto<PermissionReadDto>
            {
                Items = permissionDtos,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }
        );
    }

    public async Task<Result<PermissionReadDto>> CreatePermissionAsync(PermissionCreateDto createDto)
    {
        var permissionExists = await _unitOfWork.PermissionRepository.ExistsByNameAsync(createDto.Name);

        if (permissionExists)
        {
            return Result<PermissionReadDto>.Failure(AuthErrors.PermissionAlreadyExists);
        }

        var permission = _mapper.Map<Permission>(createDto);

        await _unitOfWork.PermissionRepository.AddPermissionAsync(permission);
        await _unitOfWork.SaveChangesAsync();

        return Result<PermissionReadDto>.Success(_mapper.Map<PermissionReadDto>(permission));
    }

    public async Task<Result<bool>> DeletePermissionAsync(int id)
    {
        var permission = await _unitOfWork.PermissionRepository.GetPermissionByIdAsync(id);

        if (permission == null)
        {
            return Result<bool>.Failure(AuthErrors.PermissionNotFound);
        }

        _unitOfWork.PermissionRepository.DeletePermission(permission);

        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}
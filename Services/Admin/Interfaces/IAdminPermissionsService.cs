using AuthService.Dtos;
using AuthService.Dtos.Permissions;
using Common;

namespace AuthService.Services.Admin.Interfaces
{
    public interface IAdminPermissionsService
    {
        Task<Result<PagedResponseDto<PermissionReadDto>>> GetPermissionsAsync(PermissionSearchDto query);
        Task<Result<PermissionReadDto>> GetPermissionByIdAsync(int id);
        Task<Result<PermissionReadDto>> CreatePermissionAsync(PermissionCreateDto createDto);
        Task<Result<bool>> DeletePermissionAsync(int id);
    }
}
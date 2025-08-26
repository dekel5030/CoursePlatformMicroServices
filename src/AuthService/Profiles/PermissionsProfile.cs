using AuthService.Dtos.Permissions;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Profiles;

public class PermissionsProfile : Profile
{
    public PermissionsProfile()
    {
        CreateMap<PermissionCreateDto, Permission>();

        CreateMap<Permission, PermissionReadDto>();
    }
}
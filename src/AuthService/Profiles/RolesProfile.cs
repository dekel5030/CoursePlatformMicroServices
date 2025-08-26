using AuthService.Dtos.Roles;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Profiles;

public class RolesProfile : Profile
{
    public RolesProfile()
    {
        CreateMap<Role, RoleReadDto>();

        CreateMap<RoleCreateDto, Role>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
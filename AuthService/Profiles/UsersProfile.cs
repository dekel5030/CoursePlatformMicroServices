using AuthService.Dtos;
using AutoMapper;

namespace AuthService.Profiles;

public class UsersProfile : Profile
{
    public UsersProfile()
    {
        CreateMap<RegisterRequestDto, UserCreateDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}
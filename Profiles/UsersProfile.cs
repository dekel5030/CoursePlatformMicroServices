using AuthService.Dtos;
using AuthService.Dtos.AuthUsers;
using AuthService.Models;
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
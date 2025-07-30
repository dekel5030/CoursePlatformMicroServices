using AuthService.Dtos;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Profiles;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<UserReadDto, UserCredentials>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}
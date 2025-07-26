using AutoMapper;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<User, UserReadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForMember(dest => dest.ProfileImageSmallUrl, opt => opt.MapFrom(src => src.ProfileImageSmallUrl))
                .ForMember(dest => dest.ProfileImageMediumUrl, opt => opt.MapFrom(src => src.ProfileImageMediumUrl))
                .ForMember(dest => dest.ProfileImageLargeUrl, opt => opt.MapFrom(src => src.ProfileImageLargeUrl))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<UserPatchDto, User>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
                .ForAllMembers(dest => dest.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ChangePasswordDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<ChangeEmailDto, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.NewEmail))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}
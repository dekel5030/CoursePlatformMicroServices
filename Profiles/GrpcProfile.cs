using AutoMapper;
using Common;
using Common.Errors;
using Common.Grpc;
using UserService.Dtos;

namespace UserService.Profiles;

public class GrpcProfile : Profile
{
    public GrpcProfile()
    {
        CreateMap<Result<UserReadDto>, Result_UserReadResponse>()
            .ForMember(dest => dest.IsSuccess, opt => opt.MapFrom(src => src.IsSuccess));

        CreateMap<UserReadDto, UserReadResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        CreateMap<Common.Errors.Error, Common.Grpc.Error>()
            .ForMember(dest => dest.HttpStatus, opt => opt.MapFrom(src => src.HttpStatus))
            .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
            .ForMember(dest => dest.MessageKey, opt => opt.MapFrom(src => src.MessageKey));

        CreateMap<Result<bool>, Result_Bool>()
            .ForMember(dest => dest.IsSuccess, opt => opt.MapFrom(src => src.IsSuccess))
            .ForMember(dest => dest.Error, opt => opt.MapFrom(src => src.Error))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

        CreateMap<UserCreateRequest, UserCreateDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash));
    }
}
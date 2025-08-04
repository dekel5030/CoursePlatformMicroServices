using AuthService.Dtos;
using AutoMapper;
using Common;
using Common.Errors;
using Common.Grpc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AuthService.Profiles;

public class GrpcProfile : Profile
{
    public GrpcProfile()
    {
        CreateMap<Common.Grpc.Error, Common.Errors.Error>()
            .ForMember(dest => dest.HttpStatus, opt => opt.MapFrom(src => src.HttpStatus))
            .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
            .ForMember(dest => dest.MessageKey, opt => opt.MapFrom(src => src.MessageKey));

        CreateMap<UserCreateDto, UserCreateRequest>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash));

        CreateMap<Result_UserReadResponse, Result<UserReadDto>>()
            .ForMember(dest => dest.IsSuccess, opt => opt.MapFrom(src => src.IsSuccess))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.Error, opt => opt.MapFrom(src => src.Error));

        CreateMap<UserReadResponse, UserReadDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        CreateMap<Result_Bool, Result<bool>>()
            .ForMember(dest => dest.IsSuccess, opt => opt.MapFrom(src => src.IsSuccess))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.Error, opt => opt.MapFrom(src => src.Error));
    }
}
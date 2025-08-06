using AutoMapper;
using CourseService.Dtos.Courses;
using CourseService.Models;

namespace CourseService.Profiles;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CourseCreateDto, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.InstructorUserId, opt => opt.MapFrom(src => src.InstructorUserId))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));
    }
}
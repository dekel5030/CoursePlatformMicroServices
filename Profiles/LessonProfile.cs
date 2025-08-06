using AutoMapper;
using CourseService.Dtos.Lessons;
using CourseService.Models;

namespace CourseService.Profiles;

public class LessonProfile : Profile
{
    public LessonProfile()
    {
        CreateMap<LessonCreateDto, Lesson>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId));
    }
}
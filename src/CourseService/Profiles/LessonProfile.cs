using AutoMapper;
using CourseService.Dtos.Lessons;
using CourseService.Models;

namespace CourseService.Profiles;

public class LessonProfile : Profile
{
    public LessonProfile()
    {
        CreateMap<LessonCreateDto, Lesson>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<Lesson, LessonReadDto>();
    }
}
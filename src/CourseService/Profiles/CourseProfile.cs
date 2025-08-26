using AutoMapper;
using CourseService.Dtos.Courses;
using CourseService.Models;

namespace CourseService.Profiles;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CourseCreateDto, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<Course, CourseReadDto>();
    }
}
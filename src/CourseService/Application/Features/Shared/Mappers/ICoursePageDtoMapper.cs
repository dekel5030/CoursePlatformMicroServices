using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;

namespace Courses.Application.Features.Shared.Mappers;

internal interface ICoursePageDtoMapper
{
    CourseDto MapCourse(Course course, CourseContext context);
    LessonDto MapLesson(Lesson lesson, CourseContext courseContext, bool hasEnrollment);
    ModuleDto MapModule(Module module, ModuleContext context);
}

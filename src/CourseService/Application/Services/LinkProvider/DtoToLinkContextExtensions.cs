using Courses.Application.Courses.Dtos;
using Courses.Application.Services.Actions.States;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;

namespace Courses.Application.Services.LinkProvider;

internal static class DtoToLinkContextExtensions
{
    public static CourseState ToCourseState(this CoursePageDto dto)
    {
        return new CourseState(
            new CourseId(dto.Id),
            new UserId(dto.InstructorId),
            dto.Status);
    }

    public static CourseState ToCourseState(this CourseSummaryDto dto)
    {
        return new CourseState(
            new CourseId(dto.Id),
            new UserId(dto.Instructor.Id),
            dto.Status);
    }

    public static ModuleState ToModuleState(this ModuleDto dto)
    {
        return new ModuleState(new ModuleId(dto.Id));
    }

    public static LessonState ToLessonState(this LessonDto dto)
    {
        return new LessonState(new LessonId(dto.Id), dto.Access);
    }

    public static ModuleLinkContext ToModuleLinkContext(this ModuleDto module, CourseState courseState)
    {
        return new ModuleLinkContext(courseState, module.ToModuleState());
    }

    public static LessonLinkContext ToLessonLinkContext(
        this LessonDto lesson,
        CourseState courseState,
        ModuleState moduleState,
        EnrollmentState? enrollmentState = null)
    {
        return new LessonLinkContext(courseState, moduleState, lesson.ToLessonState(), enrollmentState);
    }

    public static CourseCollectionContext ToCourseCollectionContext(this PagedQueryDto query, int totalCount)
    {
        return new CourseCollectionContext(query, totalCount);
    }
}

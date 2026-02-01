using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Ratings.Primitives;

namespace Courses.Application.Services.LinkProvider;

internal static class DtoToLinkContextExtensions
{
    public static CourseContext ToCourseContext(this CoursePageDto dto)
    {
        return new CourseContext(
            new CourseId(dto.Id),
            new UserId(dto.InstructorId),
            dto.Status);
    }

    public static CourseContext ToCourseContext(this CourseSummaryDto dto)
    {
        return new CourseContext(
            new CourseId(dto.Id),
            new UserId(dto.Instructor.Id),
            dto.Status);
    }

    public static ModuleContext ToModuleContext(this ModuleDto module, CourseContext courseContext)
    {
        return new ModuleContext(
            courseContext,
            new ModuleId(module.Id));
    }

    public static LessonContext ToLessonContext(
        this LessonDto lesson,
        ModuleContext moduleContext,
        bool hasEnrollment = false)
    {
        return new LessonContext(
            moduleContext,
            new LessonId(lesson.Id),
            lesson.Access,
            hasEnrollment);
    }

    public static CourseCollectionContext ToCourseCollectionContext(
        this PagedQueryDto query,
        int totalCount)
    {
        return new CourseCollectionContext(query, totalCount);
    }

    public static CourseRatingLinkContext ToRatingLinkContext(
        this CourseRatingDto dto,
        UserId? currentUserId)
    {
        return new CourseRatingLinkContext(
            new RatingId(dto.Id),
            new UserId(dto.User.Id),
            currentUserId);
    }
}

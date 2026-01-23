using Courses.Api.Contracts.Courses;
using Courses.Api.Contracts.Lessons;
using Courses.Api.Contracts.Shared;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Actions;
using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetById;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Api.Extensions;

internal static class CourseMappingExtensions
{
    public static CourseSummaryResponse ToApiContract(this CourseSummaryDto dto, LinkProvider linkProvider)
    {
        var courseContext = new CoursePolicyContext(
            dto.Id,
            dto.Instructor.Id,
            dto.Status,
            dto.LessonsCount);

        return new CourseSummaryResponse(
            dto.Id.Value,
            dto.Title.Value,
            dto.Instructor.FullName,
            dto.Instructor.Id.Value,
            dto.Instructor.AvatarUrl,
            dto.Price.Amount,
            dto.Price.Currency,
            dto.ThumbnailUrl,
            dto.LessonsCount,
            dto.EnrollmentCount,
            dto.UpdatedAtUtc,
            linkProvider.CreateCourseLinks(courseContext));
    }

    public static CourseDetailsResponse ToApiContract(this CourseDetailsDto dto, LinkProvider linkProvider)
    {
        var courseContext = new CoursePolicyContext(
            dto.Id,
            dto.Instructor.Id,
            dto.Status,
            dto.LessonsCount);

        return new CourseDetailsResponse(
            dto.Id.Value,
            dto.Title.Value,
            dto.Description.Value,
            dto.Instructor.FullName,
            dto.Instructor.Id.Value,
            dto.Instructor.AvatarUrl,
            dto.Status.ToString(),
            dto.Price.Amount,
            dto.Price.Currency,
            dto.EnrollmentCount,
            dto.LessonsCount,
            dto.UpdatedAtUtc,
            dto.ImageUrls,
            dto.Lessons.Select(lesson => lesson.ToApiContract(courseContext, linkProvider)).ToList(),
            linkProvider.CreateCourseLinks(courseContext));
    }

    public static PagedResponse<CourseSummaryResponse> ToApiContract(
        this CourseCollectionDto dto,
        LinkProvider linkProvider,
        PagedQueryDto pagedQuery)
    {
        return new PagedResponse<CourseSummaryResponse>
        {
            Items = dto.Items.Select(item => item.ToApiContract(linkProvider)).ToList(),
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalItems = dto.TotalItems,
            Links = linkProvider.CreateCourseCollectionLinks(dto, pagedQuery)
        };
    }

    public static CourseDetailsResponse ToApiContract(this CoursePageDto dto, LinkProvider linkProvider)
    {
        var courseContext = new CoursePolicyContext(
            new CourseId(dto.Id),
            new UserId(dto.InstructorId),
            dto.Status,
            dto.LessonsCount);

        var lessons = dto.Modules
            .SelectMany(
                module => module.Lessons, // 1. בחירת האוסף הפנימי
                (module, lesson) => lesson.ToApiContract(courseContext, linkProvider, new ModuleId(module.Id)) // 2. יצירת התוצאה עם גישה לשניהם
            )
            .ToList();

        return new CourseDetailsResponse(
            dto.Id,
            dto.Title,
            dto.Description,
            dto.InstructorName,
            dto.InstructorId,
            dto.InstructorAvatarUrl,
            dto.Status.ToString(),
            dto.Price.Amount,
            dto.Price.Currency,
            dto.EnrollmentCount,
            dto.LessonsCount,
            dto.UpdatedAtUtc,
            dto.ImageUrls,
            lessons,
            linkProvider.CreateCourseLinks(courseContext));
    }

    private static LessonSummaryResponse ToApiContract(
        this LessonDto lesson,
        CoursePolicyContext courseContext,
        LinkProvider linkProvider,
        ModuleId moduleId)
    {
        var lessonContext = new LessonPolicyContext(
            new Domain.Lessons.Primitives.LessonId(lesson.LessonId),
            Enum.Parse<Domain.Lessons.Primitives.LessonAccess>(lesson.Access));

        return new LessonSummaryResponse(
            lesson.LessonId,
            lesson.Title,
            lesson.Index,
            lesson.Duration,
            lesson.ThumbnailUrl,
            lesson.Access,
            linkProvider.CreateLessonLinks(courseContext, lessonContext, moduleId));
    }
}

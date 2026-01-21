using System.Linq.Expressions;
using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Lessons.Primitives;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Module;

namespace Courses.Application.Shared.Extensions;

public static class ProjectionMappings
{
    public static readonly Expression<Func<Course, IEnumerable<Module>, CourseDetailsDto>> ToCourseDetails =
        (course, modules) => new CourseDetailsDto(
            course.Id,
            course.Title,
            course.Description,
            new InstructorDto(
                course.InstructorId,
                $"{course.Instructor!.FirstName} {course.Instructor.LastName}",
                course.Instructor.AvatarUrl
            ),
            course.Status,
            course.Price,
            course.EnrollmentCount,
            modules.SelectMany(m => m.Lessons).Count(),
            course.UpdatedAtUtc,
            course.Images.Select(i => i.Path).ToList(),
            modules
                .OrderBy(m => m.Index)
                .SelectMany(m => m.Lessons.OrderBy(l => l.Index))
                .Select(ToLessonSummaryFromModule)
                .ToList()
        );

    public static Expression<Func<Course, IEnumerable<Module>, CourseSummaryDto>> ToCourseSummaryWithModules =>
        (course, modules) => new CourseSummaryDto(
            course.Id,
            course.Title,
            new InstructorDto(
                course.InstructorId,
                $"{course.Instructor!.FirstName} {course.Instructor.LastName}",
                course.Instructor.AvatarUrl
            ),
            course.Status,
            course.Price,
            course.Images.Select(i => i.Path).FirstOrDefault(),
            modules.SelectMany(m => m.Lessons).Count(),
            course.EnrollmentCount,
            course.UpdatedAtUtc
        );

    public static Expression<Func<Lesson, Module, LessonSummaryDto>> ToLessonSummaryFromModule =>
        (lesson, module) => new LessonSummaryDto(
            module.CourseId,
            module.Id,
            lesson.Id,
            lesson.Title,
            lesson.Index,
            lesson.Duration,
            lesson.ThumbnailImageUrl != null ? lesson.ThumbnailImageUrl.Path : null,
            module.Course != null && module.Course.Status == Domain.Courses.Primitives.CourseStatus.Published
                ? LessonStatus.Published
                : LessonStatus.Draft,
            lesson.Access
        );

    //public static Expression<Func<Lesson, LessonDetailsDto>> ToLessonDetails =>
    //    lesson => new LessonDetailsDto(
    //        lesson.CourseId,
    //        lesson.Id,
    //        lesson.Title,
    //        lesson.Description,
    //        lesson.Index,
    //        lesson.Duration,
    //        lesson.ThumbnailImageUrl != null ? lesson.ThumbnailImageUrl.Path : null,
    //        lesson.Access,
    //        lesson.Status,
    //        lesson.VideoUrl != null ? lesson.VideoUrl.Path : null
    //    );
}

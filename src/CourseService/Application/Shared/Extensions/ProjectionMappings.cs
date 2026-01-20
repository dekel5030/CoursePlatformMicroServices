using System.Linq.Expressions;
using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;

namespace Courses.Application.Shared.Extensions;

public static class ProjectionMappings
{
    public static readonly Expression<Func<Course, CourseDetailsDto>> ToCourseDetails =
        course => new CourseDetailsDto(
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
            course.Lessons.Count,
            course.UpdatedAtUtc,
            course.Images.Select(i => i.Path).ToList(),
            course.Lessons
                .AsQueryable()
                .OrderBy(l => l.Index)
                .Select(ToLessonSummary)
                .ToList()
        );

    public static Expression<Func<Course, CourseSummaryDto>> ToCourseSummary =>
        course => new CourseSummaryDto(
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
            course.Lessons.Count,
            course.EnrollmentCount,
            course.UpdatedAtUtc
        );

    public static Expression<Func<Lesson, LessonSummaryDto>> ToLessonSummary =>
        lesson => new LessonSummaryDto(
            lesson.CourseId,
            lesson.Id,
            lesson.Title,
            lesson.Index,
            lesson.Duration,
            lesson.ThumbnailImageUrl != null ? lesson.ThumbnailImageUrl.Path : null,
            lesson.Status,
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

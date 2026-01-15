using System.Linq;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Lessons.Extensions;
using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Lessons.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Extensions;

internal static class CourseMappingExtensions
{
    public static async Task<CourseDetailsDto> ToDetailsDtoAsync(
        this Course course,
        IStorageUrlResolver resolver,
        CancellationToken cancellationToken = default)
    {
        //var imageTasks = course.Images
        //    .Select(img => resolver.Resolve(StorageCategory.Public, img.Path))
        //    .ToList();

        //Task<List<LessonSummaryDto>> lessonsTask = course.Lessons
        //    .OrderBy(l => l.Index)
        //    .ToSummaryDtosAsync(resolver, cancellationToken);

        //await Task.WhenAll(lessonsTask);

        //return new CourseDetailsDto(
        //    Id: course.Id,
        //    Title: course.Title,
        //    Description: course.Description,
        //    InstructorName: course.InstructorId?.Value.ToString(),
        //    Price: course.Price.Amount,
        //    Currency: course.Price.Currency,
        //    EnrollmentCount: course.EnrollmentCount,
        //    UpdatedAtUtc: course.UpdatedAtUtc,
        //    ImageUrls: imageTasks.Select(t => t.Value).ToList(),
        //    Lessons: await lessonsTask,
        //    AllowedActions: new HashSet<CourseAction>()
        //);
        return new CourseDetailsDto(
            Id: course.Id,
            Title: course.Title,
            Description: course.Description,
            InstructorName: course.InstructorId?.Value.ToString(),
            Price: course.Price.Amount,
            Currency: course.Price.Currency,
            EnrollmentCount: course.EnrollmentCount,
            UpdatedAtUtc: course.UpdatedAtUtc,
            ImageUrls: course.Images,
            Lessons: course.Lessons
                .OrderBy(l => l.Index)
                .Select(lesson => new LessonSummaryDto(
                    CourseId: lesson.CourseId,
                    LessonId: lesson.Id,
                    Title: lesson.Title,
                    Description: lesson.Description,
                    Index: lesson.Index,
                    Duration: lesson.Duration,
                    IsPreview: lesson.Access == LessonAccess.Public,
                    ThumbnailUrl: lesson.ThumbnailImageUrl,
                    AllowedActions: new List<LessonAction>()
                )).ToList(),
            AllowedActions: new List<CourseAction>()
        );
    }

    public static async Task<CourseSummaryDto> ToSummaryDtoAsync(
        this Course course,
#pragma warning disable IDE0060 // Remove unused parameter
        IStorageUrlResolver resolver,
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
        CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        //string firstImagePath = course.Images.FirstOrDefault()?.Path ?? string.Empty;
        //ResolvedUrl resolvedUrl = resolver.Resolve(
        //    StorageCategory.Public, 
        //    firstImagePath);

        //return new CourseSummaryDto(
        //    Id: course.Id,
        //    Title: course.Title,
        //    InstructorName: course.InstructorId?.Value.ToString(),
        //    Price: course.Price.Amount,
        //    Currency: course.Price.Currency,
        //    ThumbnailUrl: string.IsNullOrEmpty(firstImagePath) ? null : resolvedUrl.Value,
        //    LessonsCount: course.LessonCount,
        //    EnrollmentCount: course.EnrollmentCount
        //);
        return new CourseSummaryDto(
            Id: course.Id,
            Title: course.Title,
            InstructorName: course.InstructorId?.Value.ToString(),
            Price: course.Price.Amount,
            Currency: course.Price.Currency,
            ThumbnailUrl: null,
            LessonsCount: course.LessonCount,
            EnrollmentCount: course.EnrollmentCount
        );
    }

    public static async Task<List<CourseSummaryDto>> ToSummaryDtosAsync(
        this IEnumerable<Course> courses,
        IStorageUrlResolver resolver,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Task<CourseSummaryDto>> tasks = courses
            .Select(c => c.ToSummaryDtoAsync(resolver, cancellationToken));

        CourseSummaryDto[] results = await Task.WhenAll(tasks);
        return results.ToList();
    }
}

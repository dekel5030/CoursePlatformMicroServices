using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Lessons.Extensions;
using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses;

namespace Courses.Application.Courses.Extensions;

internal static class CourseMappingExtensions
{
    public static async Task<CourseDetailsDto> ToDetailsDtoAsync(
        this Course course,
        IStorageUrlResolver resolver,
        CancellationToken cancellationToken = default)
    {
        List<Task<ResolvedUrl>> imageTasks = course.Images
            .Select(img => resolver.ResolveAsync(StorageCategory.Public, img.Path, cancellationToken))
            .ToList();

        Task<List<LessonSummaryDto>> lessonsTask = course.Lessons
            .OrderBy(l => l.Index)
            .ToSummaryDtosAsync(resolver, cancellationToken);

        await Task.WhenAll(Task.WhenAll(imageTasks), lessonsTask);

        return new CourseDetailsDto(
            Id: course.Id,
            Title: course.Title,
            Description: course.Description,
            InstructorName: course.InstructorId?.Value.ToString(),
            Price: course.Price.Amount,
            Currency: course.Price.Currency,
            EnrollmentCount: course.EnrollmentCount,
            UpdatedAtUtc: course.UpdatedAtUtc,
            ImageUrls: imageTasks.Select(t => t.Result.Value).ToList(),
            Lessons: lessonsTask.Result
        );
    }

    public static async Task<CourseSummaryDto> ToSummaryDtoAsync(
        this Course course,
        IStorageUrlResolver resolver,
        CancellationToken cancellationToken = default)
    {
        string firstImagePath = course.Images.FirstOrDefault()?.Path ?? string.Empty;
        ResolvedUrl resolvedUrl = await resolver.ResolveAsync(
            StorageCategory.Public, 
            firstImagePath, 
            cancellationToken);

        return new CourseSummaryDto(
            Id: course.Id,
            Title: course.Title,
            InstructorName: course.InstructorId?.Value.ToString(),
            Price: course.Price.Amount,
            Currency: course.Price.Currency,
            ThumbnailUrl: string.IsNullOrEmpty(firstImagePath) ? null : resolvedUrl.Value,
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

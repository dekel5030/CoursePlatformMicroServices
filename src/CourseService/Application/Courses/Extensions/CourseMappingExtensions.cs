using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Lessons.Extensions;
using Courses.Domain.Courses;

namespace Courses.Application.Courses.Extensions;

internal static class CourseMappingExtensions
{
    public static async Task<CourseDetailsDto> ToDetailsDtoAsync(
        this Course course,
        IStorageUrlResolver resolver,
        CancellationToken cancellationToken = default)
    {
        var imageTasks = course.Images
            .Select(img => resolver.ResolveAsync(StorageCategory.Public, img.Path, cancellationToken))
            .ToList();

        var lessonTasks = course.Lessons
            .OrderBy(l => l.Index)
            .Select(l => l.ToSummaryDtoAsync(resolver, cancellationToken))
            .ToList();

        await Task.WhenAll(imageTasks);
        await Task.WhenAll(lessonTasks);

        return new CourseDetailsDto(
            Id: course.Id.Value,
            Title: course.Title.Value,
            Description: course.Description.Value,
            InstructorName: course.InstructorId?.Value.ToString(),
            Price: course.Price.Amount,
            Currency: course.Price.Currency,
            EnrollmentCount: course.EnrollmentCount,
            UpdatedAtUtc: course.UpdatedAtUtc,
            ImageUrls: imageTasks.Select(t => t.Result.Value).ToList(),
            Lessons: lessonTasks.Select(t => t.Result).ToList()
        );
    }
    public static async Task<CourseSummaryDto> ToSummaryDtoAsync(
        this Course course,
        IStorageUrlResolver resolver,
        CancellationToken cancellationToken = default)
    {
        var firstImagePath = course.Images.FirstOrDefault()?.Path ?? string.Empty;
        var resolvedUrl = await resolver.ResolveAsync(StorageCategory.Public, firstImagePath, cancellationToken);

        return new CourseSummaryDto(
            Id: course.Id.Value,
            Title: course.Title.Value,
            InstructorName: course.InstructorId?.Value.ToString(),
            Price: course.Price.Amount,
            Currency: course.Price.Currency,
            ThumbnailUrl: resolvedUrl.Value,
            LessonsCount: course.LessonCount,
            EnrollmentCount: course.EnrollmentCount
        );
    }

    public static async Task<List<CourseSummaryDto>> ToSummaryDtosAsync(
        this IEnumerable<Course> courses,
        IStorageUrlResolver resolver,
        CancellationToken cancellationToken = default)
    {
        var tasks = courses.Select(c => c.ToSummaryDtoAsync(resolver, cancellationToken));
        return (await Task.WhenAll(tasks)).ToList();
    }
}

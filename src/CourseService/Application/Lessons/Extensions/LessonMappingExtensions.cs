using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Lessons.Extensions;

internal static class LessonMappingExtensions
{
    public static async Task<LessonSummaryDto> ToSummaryDtoAsync(
        this Lesson lesson,
#pragma warning disable IDE0060 // Remove unused parameter
        IStorageUrlResolver resolver,
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
        CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        //Uri? thumbnailUrl = lesson.ThumbnailImageUrl != null
        //    ? (await resolver.Resolve(
        //        StorageCategory.Public, 
        //        lesson.ThumbnailImageUrl.Path, 
        //        cancellationToken)).Value
        //    : null;

        //return new LessonSummaryDto(
        //    CourseId: lesson.CourseId,
        //    LessonId: lesson.Id,
        //    Title: lesson.Title,
        //    Description: lesson.Description,
        //    Index: lesson.Index,
        //    Duration: lesson.Duration,
        //    IsPreview: lesson.Access == LessonAccess.Public,
        //    ThumbnailUrl: thumbnailUrl
        //);
        return new LessonSummaryDto(
                CourseId: lesson.CourseId,
                LessonId: lesson.Id,
                Title: lesson.Title,
                Description: lesson.Description,
                Index: lesson.Index,
                Duration: lesson.Duration,
                IsPreview: lesson.Access == LessonAccess.Public,
                ThumbnailUrl: lesson.ThumbnailImageUrl,
                AllowedActions: new List<LessonAction>()
        );
    }

    public static async Task<List<LessonSummaryDto>> ToSummaryDtosAsync(
        this IEnumerable<Lesson> lessons,
        IStorageUrlResolver resolver,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Task<LessonSummaryDto>> tasks = lessons
            .Select(l => l.ToSummaryDtoAsync(resolver, cancellationToken));

        LessonSummaryDto[] results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    public static async Task<LessonDetailsDto> ToDetailsDtoAsync(
        this Lesson lesson,
#pragma warning disable IDE0060 // Remove unused parameter
        IStorageUrlResolver resolver,
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning disable IDE0060 // Remove unused parameter
        CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        //Task<ResolvedUrl>? thumbTask = lesson.ThumbnailImageUrl != null
        //    ? resolver.ResolveAsync(StorageCategory.Public, lesson.ThumbnailImageUrl.Path, cancellationToken)
        //    : null;

        //Task<ResolvedUrl>? videoTask = lesson.VideoUrl != null
        //    ? resolver.ResolveAsync(StorageCategory.Private, lesson.VideoUrl.Path, cancellationToken)
        //    : null;

        //IEnumerable<Task<ResolvedUrl>> tasks = new[] { thumbTask, videoTask }
        //    .Where(t => t != null).Cast<Task<ResolvedUrl>>();

        //await Task.WhenAll(tasks);

        //return new LessonDetailsDto(
        //    CourseId: lesson.CourseId,
        //    LessonId: lesson.Id,
        //    Title: lesson.Title,
        //    Description: lesson.Description,
        //    Index: lesson.Index,
        //    Duration: lesson.Duration,
        //    IsPreview: lesson.Access == LessonAccess.Public,
        //    ThumbnailUrl: thumbTask != null ? (await thumbTask).Value : null,
        //    VideoUrl: videoTask != null ? (await videoTask).Value : null
        //);

        return new LessonDetailsDto(
            CourseId: lesson.CourseId,
            LessonId: lesson.Id,
            Title: lesson.Title,
            Description: lesson.Description,
            Index: lesson.Index,
            Duration: lesson.Duration,
            IsPreview: lesson.Access == LessonAccess.Public,
            ThumbnailUrl: lesson.ThumbnailImageUrl,
            VideoUrl: lesson.VideoUrl
        );
    }
}

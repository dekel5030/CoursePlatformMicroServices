using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Lessons.Extensions;

internal static class LessonMappingExtensions
{
    public static async Task<LessonSummaryDto> ToSummaryDtoAsync(
        this Lesson lesson,
        IStorageUrlResolver resolver,
        CancellationToken cancellationToken = default)
    {
        var thumbnailUrl = lesson.ThumbnailImageUrl != null
            ? (await resolver.ResolveAsync(StorageCategory.Public, lesson.ThumbnailImageUrl.Path, cancellationToken)).Value
            : null;

        return new LessonSummaryDto(
            Id: lesson.Id.Value,
            Title: lesson.Title.Value,
            Description: lesson.Description.Value,
            Index: lesson.Index,
            Duration: lesson.Duration,
            IsPreview: lesson.Access == LessonAccess.Public,
            ThumbnailUrl: thumbnailUrl
        );
    }

    public static async Task<LessonDetailsDto> ToDetailsDtoAsync(
        this Lesson lesson,
        IStorageUrlResolver resolver,
        CancellationToken cancellationToken = default)
    {
        var thumbTask = resolver.ResolveAsync(StorageCategory.Public, lesson.ThumbnailImageUrl?.Path ?? string.Empty, cancellationToken);
        var videoTask = resolver.ResolveAsync(StorageCategory.Private, lesson.VideoUrl?.Path ?? string.Empty, cancellationToken);

        await Task.WhenAll(thumbTask, videoTask);

        return new LessonDetailsDto(
            Id: lesson.Id.Value,
            Title: lesson.Title.Value,
            Description: lesson.Description.Value,
            Index: lesson.Index,
            Duration: lesson.Duration,
            IsPreview: lesson.Access == LessonAccess.Public,
            ThumbnailUrl: thumbTask.Result.Value,
            VideoUrl: videoTask.Result.Value
        );
    }
}
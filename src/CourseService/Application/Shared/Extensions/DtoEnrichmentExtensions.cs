using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Shared.Dtos;

namespace Courses.Application.Shared.Extensions;

public static class DtoEnrichmentExtensions
{
    public static CourseDetailsDto EnrichWithUrls(this CourseDetailsDto dto, IStorageUrlResolver resolver)
    {
        return dto with
        {
            Instructor = dto.Instructor.Enrich(resolver),
            ImageUrls = dto.ImageUrls.ResolveAll(resolver),
            Lessons = dto.Lessons.Select(l => l.EnrichWithUrls(resolver)).ToList()
        };
    }

    public static CourseSummaryDto EnrichWithUrls(this CourseSummaryDto dto, IStorageUrlResolver resolver)
    {
        return dto with
        {
            Instructor = dto.Instructor.Enrich(resolver),
            ThumbnailUrl = resolver.ResolveOptional(dto.ThumbnailUrl)
        };
    }

    public static CourseCollectionDto EnrichWithUrls(this CourseCollectionDto collection, IStorageUrlResolver resolver)
    {
        var enrichedItems = collection.Items
            .Select(item => item.EnrichWithUrls(resolver))
            .ToList();

        return collection with { Items = enrichedItems };
    }

    public static LessonSummaryDto EnrichWithUrls(this LessonSummaryDto dto, IStorageUrlResolver resolver)
    {
        return dto with
        {
            ThumbnailUrl = resolver.ResolveOptional(dto.ThumbnailUrl)
        };
    }

    public static LessonDetailsDto EnrichWithUrls(this LessonDetailsDto dto, IStorageUrlResolver resolver)
    {
        return dto with
        {
            ThumbnailUrl = resolver.ResolveOptional(dto.ThumbnailUrl),
            VideoUrl = resolver.ResolveOptional(dto.VideoUrl)
        };
    }
    private static InstructorDto Enrich(this InstructorDto dto, IStorageUrlResolver resolver)
    {
        return dto with { AvatarUrl = resolver.ResolveOptional(dto.AvatarUrl) };
    }

    private static string? ResolveOptional(this IStorageUrlResolver resolver, string? path)
    {
        return path is not null ? resolver.Resolve(StorageCategory.Public, path).Value : null;
    }

    private static List<string> ResolveAll(this IEnumerable<string> paths, IStorageUrlResolver resolver)
    {
        return paths
            .Select(path => resolver.Resolve(StorageCategory.Public, path).Value)
            .Where(url => url is not null)
            .ToList()!;
    }
}

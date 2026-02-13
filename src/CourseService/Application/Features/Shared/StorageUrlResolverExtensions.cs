using Courses.Application.Abstractions.Storage;

namespace Courses.Application.Features.Shared;

public static class StorageUrlResolverExtensions
{
    public static string? ResolvePublicUrl(this IStorageUrlResolver resolver, string? path)
    {
        return path is not null ? resolver.Resolve(StorageCategory.Public, path).Value : null;
    }
}

namespace Courses.Application.Abstractions.Storage;

public interface IStorageUrlResolver
{
    Task<ResolvedUrl> ResolveAsync(StorageCategory category, string relativePath);
}

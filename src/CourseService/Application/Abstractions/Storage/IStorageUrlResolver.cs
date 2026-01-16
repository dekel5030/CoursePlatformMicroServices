namespace Courses.Application.Abstractions.Storage;

public interface IStorageUrlResolver
{
    ResolvedUrl Resolve(
        StorageCategory category,
        string relativePath);
}

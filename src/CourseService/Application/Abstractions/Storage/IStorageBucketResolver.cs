namespace Courses.Application.Abstractions.Storage;

public interface IStorageBucketResolver
{
    string GetBucket(StorageCategory category);
}

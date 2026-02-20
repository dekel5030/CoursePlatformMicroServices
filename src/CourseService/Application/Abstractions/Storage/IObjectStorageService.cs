namespace Courses.Application.Abstractions.Storage;

public interface IObjectStorageService
{
    PresignedUrlResponse GenerateUploadUrlAsync(
        StorageCategory category,
        string fileKey,
        string referenceId,
        string referenceType,
        TimeSpan? expiry = null,
        Dictionary<string, string>? metadata = null);

    PresignedUrlResponse GenerateViewUrl(string fileKey, TimeSpan expiry);
}

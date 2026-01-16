namespace Courses.Application.Abstractions.Storage;

public interface IObjectStorageService
{
    PresignedUrlResponse GenerateUploadUrlAsync(
        StorageCategory category,
        string fileKey,
        string referenceId,
        string referenceType,
        TimeSpan? expiry = null);

    PresignedUrlResponse GenerateViewUrl(string fileKey, TimeSpan expiry);
}

public record PresignedUrlResponse(string Url, string FileKey, DateTime ExpiresAt);

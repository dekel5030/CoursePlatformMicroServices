namespace Courses.Application.Abstractions.Storage;

public interface IObjectStorageService
{
    PresignedUrlResponse GenerateUploadUrl(string fileName, string contentType, TimeSpan expiry);

    PresignedUrlResponse GenerateViewUrl(string fileKey, TimeSpan expiry);
    Task<string> UploadFileAsync(string fileName, Stream stream, string contentType);

}

public record PresignedUrlResponse(string Url, string FileKey);
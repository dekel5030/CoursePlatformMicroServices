namespace StorageService.Abstractions;

public interface IStorageProvider
{
    PresignedUrlResponse GenerateViewUrl(string fileKey, TimeSpan expiry);

    Task<string> UploadFileAsync(Stream stream, string fileKey, string contentType, long contentLength);
    Task<bool> DeleteFileAsync(string fileKey);
}
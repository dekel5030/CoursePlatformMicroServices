using Kernel;

namespace StorageService.Abstractions;

public interface IStorageProvider
{
    PresignedUrlResponse GenerateViewUrl(string fileKey, TimeSpan expiry);

    Task<Result<string>> UploadObjectAsync(
        Stream stream, 
        string fileKey, 
        string contentType, 
        long contentLength, 
        string bucket);
    Task<bool> DeleteFileAsync(string fileKey);
    Task<ObjectResponse> GetObjectAsync(string bucketName, string key, CancellationToken cancellationToken = default);

}

public class ObjectResponse
{
    public Stream Content { get; set; } = default!;
    public string ContentType { get; set; } = "application/octet-stream";
    public long? ContentLength { get; set; }
}
using Kernel;

namespace StorageService.Abstractions;

internal interface IStorageProvider
{
    PresignedUrlResponse GenerateViewUrl(string fileKey, TimeSpan expiry);

    Task<Result<string>> UploadObjectAsync(
        Stream stream,
        string fileKey,
        string contentType,
        long contentLength,
        string bucket,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileKey);
    Task<ObjectResponse> GetObjectAsync(string bucketName, string key, CancellationToken cancellationToken = default);

}

internal sealed class ObjectResponse
{
    public Stream Content { get; set; } = default!;
    public string ContentType { get; set; } = "application/octet-stream";
    public long? ContentLength { get; set; }
    public string? ETag { get; set; }
}

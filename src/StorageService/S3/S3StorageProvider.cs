using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using StorageService.Abstractions;

namespace StorageService.S3;

public class S3StorageProvider : IStorageProvider
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Options _options;

    public S3StorageProvider(IAmazonS3 s3Client, IOptions<S3Options> options)
    {
        _s3Client = s3Client;
        _options = options.Value;
    }

    public PresignedUrlResponse GenerateViewUrl(string fileKey, TimeSpan expiry)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = fileKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(expiry)
        };

        var url = _s3Client.GetPreSignedURL(request);

        if (!string.IsNullOrEmpty(_options.PublicUrl))
        {
            var uriBuilder = new UriBuilder(url);
            var publicUri = new Uri(_options.PublicUrl);

            uriBuilder.Scheme = publicUri.Scheme;
            uriBuilder.Host = publicUri.Host;
            uriBuilder.Port = publicUri.Port;

            url = uriBuilder.ToString();
        }

        return new PresignedUrlResponse(url, fileKey, request.Expires.Value);
    }
    public async Task<string> UploadFileAsync(Stream stream, string fileKey, string contentType, long contentLength)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ms.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = fileKey,
            InputStream = ms,
            ContentType = contentType,
            UseChunkEncoding = false,
        };

        await _s3Client.PutObjectAsync(request);
        return fileKey;
    }

    public async Task<bool> DeleteFileAsync(string fileKey)
    {
        try
        {
            await _s3Client.DeleteObjectAsync(_options.BucketName, fileKey);
            return true;
        }
        catch (AmazonS3Exception)
        {
            return false;
        }
    }
}

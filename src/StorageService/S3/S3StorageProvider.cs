using Amazon.S3;
using Amazon.S3.Model;
using Kernel;
using Microsoft.Extensions.Options;
using StorageService.Abstractions;

namespace StorageService.S3;

internal sealed class S3StorageProvider : IStorageProvider
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
    public async Task<Result<string>> UploadObjectAsync(
        Stream stream, 
        string fileKey, 
        string contentType, 
        long contentLength,
        string bucket)
    {
        var tempFilePath = Path.GetRandomFileName();

        try
        {
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }

            using var uploadStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);
            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = fileKey,
                InputStream = uploadStream,
                ContentType = contentType,
                UseChunkEncoding = false,
            };

            await _s3Client.PutObjectAsync(request);
        }
        catch (AmazonS3Exception)
        {
            return Result.Failure<string>(Error.Problem("s3.upload_failed", "Storage service rejected the request."));
        }
        catch (IOException)
        {
            return Result.Failure<string>(Error.Problem("s3.io_error", "Internal server storage error."));
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }

        return Result.Success(fileKey);
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

    public async Task<ObjectResponse> GetObjectAsync(
        string bucketName, 
        string key, 
        CancellationToken cancellationToken = default)
    {
        var response = await _s3Client.GetObjectAsync(bucketName, key, cancellationToken);

        return new ObjectResponse
        {
            Content = response.ResponseStream,
            ContentType = response.Headers.ContentType ?? "application/octet-stream",
            ContentLength = response.ContentLength
        };
    }
}

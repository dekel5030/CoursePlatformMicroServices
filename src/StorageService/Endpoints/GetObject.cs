using System.IO;
using CoursePlatform.ServiceDefaults.Endpoints;
using MassTransit;
using Microsoft.Extensions.Primitives;
using StorageService.Abstractions;

namespace StorageService.Endpoints;

internal sealed class GetObject : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("storage/{bucketName}/{*key}", async (
            string key,
            string bucketName,
            HttpResponse response,
            HttpRequest request,
            IStorageProvider storage) =>
        {
            ObjectResponse file = await storage.GetObjectAsync(bucketName, key);

            string etag = $"\"{file.ETag}\"";

            response.Headers["Cache-Control"] = "public, max-age=31536000, immutable";
            response.Headers["ETag"] = etag;

            if (request.Headers.TryGetValue("If-None-Match", out StringValues value)
                && value.Any(v => v?.Split(',').Select(x => x.Trim()).Contains(etag) ?? false))
            {
                return Results.StatusCode(StatusCodes.Status304NotModified);
            }

            return Results.File(
                fileStream: file.Content,
                contentType: file.ContentType,
                enableRangeProcessing: true
            );
        });
    }
}

using CoursePlatform.ServiceDefaults.Endpoints;
using Microsoft.AspNetCore.StaticFiles; // תוודא שיש לך את זה או תשתמש בפונקציית עזר ידנית
using Microsoft.Extensions.Primitives;
using StorageService.Abstractions;

namespace StorageService.Endpoints;

internal sealed class GetObject : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        string routePattern = "storage/{bucketName}/{*key}";

        app.MapMethods(routePattern, ["OPTIONS"], (HttpResponse response) =>
        {
            response.Headers.Append("Access-Control-Allow-Origin", "*");
            response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
            response.Headers.Append("Access-Control-Allow-Headers", "*");
            return Results.Ok();
        });

        app.MapGet(routePattern, async (
            string key,
            string bucketName,
            HttpResponse response,
            HttpRequest request,
            IStorageProvider storage) =>
        {
            response.Headers.Append("Access-Control-Allow-Origin", "*");
            response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
            response.Headers.Append("Access-Control-Allow-Headers", "*");

            ObjectResponse file = await storage.GetObjectAsync(bucketName, key);

            string etag = $"\"{file.ETag}\"";
            response.Headers.CacheControl = "public, max-age=31536000, immutable";
            response.Headers.ETag = etag;

            if (request.Headers.TryGetValue("If-None-Match", out StringValues value)
                && value.Any(v => v?.Split(',').Select(x => x.Trim()).Contains(etag) ?? false))
            {
                return Results.StatusCode(StatusCodes.Status304NotModified);
            }

            string contentType = GetContentType(key, file.ContentType);

            return Results.File(
                fileStream: file.Content,
                contentType: contentType,
                enableRangeProcessing: true
            );
        });
    }

    private static string GetContentType(string key, string originalContentType)
    {
        if (key.EndsWith(".m3u8", StringComparison.OrdinalIgnoreCase))
        {
            return "application/x-mpegURL";
        }

        if (key.EndsWith(".ts", StringComparison.OrdinalIgnoreCase))
        {
            return "video/MP2T";
        }

        return originalContentType ?? "application/octet-stream";
    }
}

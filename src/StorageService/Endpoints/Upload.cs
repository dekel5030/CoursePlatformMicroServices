using CoursePlatform.ServiceDefaults.Endpoints;
using Microsoft.AspNetCore.Mvc;
using StorageService.Abstractions;

namespace StorageService.Endpoints;

public class Upload : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("storage/upload/{key}", async (
            string key,
            HttpRequest request,
            IStorageProvider storage) =>
        {
            if (!request.HasFormContentType && request.ContentLength == 0)
                return Results.BadRequest("No file content.");

            var fileKey = await storage.UploadFileAsync(
                request.Body,
                key,
                request.ContentType ?? "application/octet-stream",
                request.ContentLength ?? throw new ArgumentNullException(nameof(request.ContentLength)));

            return Results.Ok(new { Key = fileKey });
        })
        .WithMetadata(new DisableRequestSizeLimitAttribute());
    }
}
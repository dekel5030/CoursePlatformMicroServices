using CoursePlatform.ServiceDefaults.Endpoints;
using StorageService.Abstractions;

namespace StorageService.Endpoints;

public class GetObject : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("storage/{bucketName}/{*key}", async (
            string key,
            string bucketName,
            IStorageProvider storage) =>
        {
            var response = await storage.GetObjectAsync(bucketName, key);

            return Results.File(
                fileStream: response.Content,
                contentType: response.ContentType,
                enableRangeProcessing: true
            );
        });
    }
}
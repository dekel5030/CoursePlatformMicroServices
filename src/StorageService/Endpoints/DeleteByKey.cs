using CoursePlatform.ServiceDefaults.Endpoints;
using StorageService.Abstractions;

namespace StorageService.Endpoints;

public class DeleteByKey : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("storage/{key}", async (
            string key,
            IStorageProvider storage) =>
        {
            var deleted = await storage.DeleteFileAsync(key);
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }
}
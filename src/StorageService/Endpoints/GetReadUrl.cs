using CoursePlatform.ServiceDefaults.Endpoints;
using StorageService.Abstractions;

namespace StorageService.Endpoints;

public class GetReadUrl : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("storage/url/{key}", async (
            string key,
            IStorageProvider storage) =>
        {
            var url = storage.GenerateViewUrl(key, TimeSpan.FromMinutes(30));
            return Results.Ok(new { Url = url });
        });
    }
}
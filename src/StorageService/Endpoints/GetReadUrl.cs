using CoursePlatform.ServiceDefaults.Endpoints;
using StorageService.Abstractions;

namespace StorageService.Endpoints;

internal sealed class GetReadUrl : IEndpoint
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

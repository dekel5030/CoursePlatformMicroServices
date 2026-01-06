using System.Text;
using Courses.Application.Abstractions.ObjectStorage;

namespace Courses.Api.Endpoints.S3;

public class TestUpload : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses/upload", async (
            IObjectStorageService storageService) =>
        {
            var content = "Hello Garage! This is a test file from Aspire CourseService.";
            var bytes = Encoding.UTF8.GetBytes(content);
            using var stream = new MemoryStream(bytes);

            try
            {
                var fileKey = await storageService.UploadFileAsync("test.txt", stream, "text/plain");

                var viewUrl = storageService.GenerateViewUrl(fileKey, TimeSpan.FromMinutes(10));

                return Results.Ok(new { FileKey = fileKey, PreviewUrl = viewUrl });
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Upload failed: {ex.Message}");
            }
        });
    }
}

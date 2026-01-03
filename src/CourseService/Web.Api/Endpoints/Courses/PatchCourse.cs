using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Commands.PatchCourse;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

public class PatchCourse : IEndpoint
{
    public record PatchCourseRequest(
        string? Title,
        string? Description,
        Guid? InstructorId,
        decimal? PriceAmount,
        string? PriceCurrency);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("courses/{id:guid}", async (
            Guid id,
            PatchCourseRequest request,
            IMediator mediator) =>
        {
            var command = new PatchCourseCommand(
                id,
                request.Title,
                request.Description,
                request.InstructorId,
                request.PriceAmount,
                request.PriceCurrency);

            Result result = await mediator.Send(command);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithName(nameof(PatchCourse))
        .WithTags(Tags.Courses)
        .WithSummary("Partially updates a course with only the fields provided.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

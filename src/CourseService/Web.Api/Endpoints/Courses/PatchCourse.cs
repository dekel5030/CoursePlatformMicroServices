using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Commands.PatchCourse;
using Kernel;
using Kernel.Messaging.Abstractions;
using CoursePlatform.ServiceDefaults.Swagger;
using Microsoft.AspNetCore.Mvc;

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
        .WithMetadata<EmptyResult>(nameof(PatchCourse), Tags.Courses, "Patch Course");
    }
}

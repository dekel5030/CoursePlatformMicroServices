using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Commands.PatchCourse;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Courses;

internal sealed class PatchCourse : IEndpoint
{
    internal sealed record PatchCourseRequest(
        string? Title,
        string? Description,
        Guid? InstructorId,
        decimal? PriceAmount,
        string? PriceCurrency);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("courses/{id:Guid}", async (
            Guid id,
            PatchCourseRequest request,
            IMediator mediator) =>
        {
            Title? title = string.IsNullOrWhiteSpace(request.Title) ? null : new Title(request.Title);
            Description? description = string.IsNullOrWhiteSpace(request.Description) ? null : new Description(request.Description);

            var command = new PatchCourseCommand(
                new CourseId(id),
                title,
                description,
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

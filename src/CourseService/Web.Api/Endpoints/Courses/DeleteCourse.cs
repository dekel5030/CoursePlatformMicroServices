using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Commands.DeleteCourse;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Courses;

public class DeleteCourse : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("courses/{courseId}", async (
            [FromRoute] CourseId courseId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteCourseCommand(courseId);
            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(nameof(DeleteCourse), Tags.Courses, "Delete course", 204);
    }
}

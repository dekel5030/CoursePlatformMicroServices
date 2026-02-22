using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Tasks.Dtos;
using Courses.Application.Tasks.Queries.GetAllTasks;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Tasks;

internal sealed class GetAllTasks : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("tasks", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllTasksQuery();
            Result<IReadOnlyList<MediaProcessingTaskDto>> result =
                await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithMetadata<IReadOnlyList<MediaProcessingTaskDto>>(
            nameof(GetAllTasks),
            tag: Tags.Tasks,
            summary: "Gets all media processing tasks.");
    }
}

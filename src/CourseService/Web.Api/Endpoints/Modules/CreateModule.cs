using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Modules.Commands.CreateModule;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Modules;

internal sealed class CreateModule : IEndpoint
{
    internal sealed record CreateModuleRequest(string? Title);

    internal sealed record CreateResponse(Guid ModuleId, Guid CourseId, string Title);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses/{courseId:Guid}/modules", async (
            Guid courseId,
            CreateModuleRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            Title? title = string.IsNullOrWhiteSpace(request.Title) ? null : new Title(request.Title);
            var courseIdObj = new CourseId(courseId);

            var command = new CreateModuleCommand(courseIdObj, title);

            Result<CreateModuleResponse> result = await mediator.Send(command, cancellationToken);

            return result.Match(
                module => Results.CreatedAtRoute(
                    nameof(GetModulesByCourseId),
                    new { courseId = module.CourseId },
                    new CreateResponse(module.ModuleId, module.CourseId, module.Title)
                ),
                CustomResults.Problem);
        })
        .WithMetadata<CreateResponse>(
            nameof(CreateModule),
            tag: Tags.Modules,
            summary: "Creates a new module for a course.",
            successStatusCode: StatusCodes.Status201Created);
    }
}

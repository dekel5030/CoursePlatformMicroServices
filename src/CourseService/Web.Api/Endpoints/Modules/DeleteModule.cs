using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Modules.Commands.DeleteModule;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Modules;

internal sealed class DeleteModule : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("modules/{moduleId:Guid}", async (
            Guid moduleId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteModuleCommand(new ModuleId(moduleId));
            Result result = await mediator.Send(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(
            nameof(DeleteModule),
            Tags.Modules,
            "Deletes a module from a course.",
            204);
    }
}

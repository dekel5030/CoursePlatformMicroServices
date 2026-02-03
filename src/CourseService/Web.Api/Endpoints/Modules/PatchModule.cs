using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Modules.Commands.PatchModule;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Modules;

internal sealed class PatchModule : IEndpoint
{
    internal sealed record PatchModuleRequest(string? Title);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("modules/{moduleId:Guid}", async (
            Guid moduleId,
            PatchModuleRequest request,
            IMediator mediator) =>
        {
            Title? title = string.IsNullOrWhiteSpace(request.Title) ? null : new Title(request.Title);

            var command = new PatchModuleCommand(new ModuleId(moduleId), title);

            Result result = await mediator.Send(command);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(
            nameof(PatchModule),
            Tags.Modules,
            "Partially updates a module with only the fields provided.");
    }
}

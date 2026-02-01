using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Categories.Commands.DeleteCategory;
using Courses.Domain.Categories.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Categories;

internal sealed class DeleteCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("categories/{id:Guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteCategoryCommand(new CategoryId(id));
            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(nameof(DeleteCategory), Tags.Categories, "Deletes a category.", 204);
    }
}

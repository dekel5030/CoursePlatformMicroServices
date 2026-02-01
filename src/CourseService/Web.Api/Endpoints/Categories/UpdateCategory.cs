using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Categories.Commands.UpdateCategory;
using Courses.Domain.Categories.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Categories;

internal sealed class UpdateCategory : IEndpoint
{
    internal sealed record UpdateCategoryRequest(string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("categories/{id:Guid}", async (
            Guid id,
            UpdateCategoryRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCategoryCommand(new CategoryId(id), request.Name);

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(nameof(UpdateCategory), Tags.Categories, "Updates a category.");
    }
}

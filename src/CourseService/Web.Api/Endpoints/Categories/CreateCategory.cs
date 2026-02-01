using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Categories.Commands.CreateCategory;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Categories;

internal sealed class CreateCategory : IEndpoint
{
    internal sealed record CreateCategoryRequest(string Name);

    internal sealed record CreateResponse(Guid Id, string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("categories", async (
            CreateCategoryRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateCategoryCommand(request.Name);

            Result<CreateCategoryResponse> result = await mediator.Send(command, cancellationToken);

            return result.Match(
                response => Results.Created($"/categories/{response.Id}", new CreateResponse(response.Id, response.Name)),
                CustomResults.Problem);
        })
        .WithMetadata<CreateResponse>(
            nameof(CreateCategory),
            tag: Tags.Categories,
            summary: "Creates a new category.",
            successStatusCode: StatusCodes.Status201Created);
    }
}

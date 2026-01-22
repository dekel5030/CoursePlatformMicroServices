using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Contracts.Categories;
using Courses.Api.Contracts.Shared;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Categories.Dtos;
using Courses.Application.Categories.Queries.GetCategories;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Categories;

internal sealed class GetCategories : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("categories", async (
            IMediator mediator,
            LinkProvider linkProvider,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCategoriesQuery();
            Result<CategoryCollectionDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto.ToApiContract(linkProvider)),
                CustomResults.Problem);
        })
        .WithMetadata<PagedResponse<CategoryResponse>>(
            nameof(GetCategories),
            tag: Tags.Categories,
            summary: "Gets all categories.");
    }
}

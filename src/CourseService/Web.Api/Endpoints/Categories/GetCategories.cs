using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
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
            CancellationToken cancellationToken) =>
        {
            var query = new GetCategoriesQuery(new CategoryFilter());
            Result<IReadOnlyList<CategoryDto>> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                list =>
                {
                    var dto = new CategoryCollectionDto
                    {
                        Items = list,
                        PageNumber = 1,
                        PageSize = list.Count,
                        TotalItems = list.Count,
                        Links = null
                    };
                    return Results.Ok(dto);
                },
                CustomResults.Problem);
        })
        .WithMetadata<CategoryCollectionDto>(
            nameof(GetCategories),
            tag: Tags.Categories,
            summary: "Gets all categories.");
    }
}

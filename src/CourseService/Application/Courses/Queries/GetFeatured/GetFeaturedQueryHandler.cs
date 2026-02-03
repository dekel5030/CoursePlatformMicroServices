using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetFeaturedCourseSummaries;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Shared.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetFeatured;

internal sealed class GetFeaturedQueryHandler : IQueryHandler<GetFeaturedQuery, CourseCollectionDto>
{
    private readonly ILinkBuilderService _linkBuilder;
    private readonly IMediator _mediator;

    public GetFeaturedQueryHandler(
        ILinkBuilderService linkBuilder,
        IMediator mediator)
    {
        _linkBuilder = linkBuilder;
        _mediator = mediator;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetFeaturedQuery request,
        CancellationToken cancellationToken = default)
    {
        var innerQuery = new GetFeaturedCourseSummariesQuery();
        Result<CourseCollectionDto> innerQueryResult = await _mediator.Send(innerQuery, cancellationToken);

        if (innerQueryResult.IsFailure)
        {
            return innerQueryResult;
        }

        CourseCollectionDto dto = innerQueryResult.Value;
        CourseCollectionDto enrichedDto = dto.EnrichWithFeaturedLinks(_linkBuilder);

        return Result.Success(enrichedDto);
    }
}

internal static class FeaturedCourseCollectionDtoEnrichmentExtensions
{
    public static CourseCollectionDto EnrichWithFeaturedLinks(
        this CourseCollectionDto dto,
        ILinkBuilderService linkBuilder)
    {
        var enrichedItems = dto.Items.Select(courseSummaryDto =>
        {
            var courseContext = courseSummaryDto.ToCourseContext();
            IReadOnlyList<LinkDto> links = linkBuilder.BuildLinks(LinkResourceKey.Course, courseContext);

            return courseSummaryDto with { Course = courseSummaryDto.Course with { Links = links } };
        }).ToList();

        var pagedQuery = new PagedQueryDto
        {
            Page = dto.PageNumber,
            PageSize = dto.PageSize
        };

        var collectionContext = pagedQuery.ToCourseCollectionContext(dto.TotalItems);

        IReadOnlyList<LinkDto> collectionLinks = linkBuilder.BuildLinks(
            LinkResourceKey.CourseCollection,
            collectionContext);

        return dto with
        {
            Items = enrichedItems,
            Links = collectionLinks.ToList()
        };
    }
}

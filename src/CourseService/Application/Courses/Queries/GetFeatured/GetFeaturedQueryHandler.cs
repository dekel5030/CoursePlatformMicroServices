using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetFeaturedCourseSummaries;
using Courses.Application.Services.Actions.States;
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
        var courseDtos = dto.Items.Select(courseDto =>
        {
            var courseState = courseDto.ToCourseState();
            IReadOnlyList<LinkDto> links = linkBuilder.BuildLinks(LinkResourceKey.Course, courseState);
            return courseDto with { Links = links };
        }).ToList();

        var collectionContext = new PagedQueryDto
            { Page = dto.PageNumber, PageSize = dto.PageSize }.ToCourseCollectionContext(dto.TotalItems);
        IReadOnlyList<LinkDto> collectionLinks = linkBuilder.BuildLinks(
            LinkResourceKey.CourseCollection,
            collectionContext);

        return new CourseCollectionDto
        {
            Items = courseDtos,
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalItems = dto.TotalItems,
            Links = collectionLinks.ToList()
        };
    }
}

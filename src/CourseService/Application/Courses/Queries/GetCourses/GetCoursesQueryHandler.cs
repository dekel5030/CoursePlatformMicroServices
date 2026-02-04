using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetCourseCacheable;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Shared.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCourses;

internal sealed class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, CourseCollectionDto>
{
    private readonly ILinkBuilderService _linkBuilder;
    private readonly IMediator _mediator;

    public GetCoursesQueryHandler(
        ILinkBuilderService linkBuilder,
        IMediator mediator)
    {
        _linkBuilder = linkBuilder;
        _mediator = mediator;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        var innerQuery = new GetCourseCacheableQuery(request.PagedQuery);
        Result<CourseCollectionDto> innerQueryResult = await _mediator.Send(innerQuery, cancellationToken);

        if (innerQueryResult.IsFailure)
        {
            return innerQueryResult;
        }

        CourseCollectionDto dto = innerQueryResult.Value;
        CourseCollectionDto enrichedDto = dto.EnrichWithLinks(_linkBuilder, request.PagedQuery);

        return Result.Success(enrichedDto);
    }
}

internal static class CourseCollectionDtoEnrichmentExtensions
{
    public static CourseCollectionDto EnrichWithLinks(
        this CourseCollectionDto dto,
        ILinkBuilderService linkBuilder,
        PagedQueryDto pagedQuery)
    {
        var courseDtos = dto.Items.Select(courseDto =>
        {
            var courseContext = courseDto.ToCourseContext();
            IReadOnlyList<LinkDto> links = linkBuilder.BuildLinks(LinkResourceKey.Course, courseContext);

            return courseDto with { Course = courseDto.Course with { Links = links } };
        }).ToList();

        var collectionContext = (pagedQuery with
        { Page = dto.PageNumber, PageSize = dto.PageSize }).ToCourseCollectionContext(dto.TotalItems);
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

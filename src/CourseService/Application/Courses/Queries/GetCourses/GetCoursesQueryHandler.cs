using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetCourseSummaries;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCourses;

internal sealed class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, CourseCollectionDto>
{
    private readonly ICourseLinkFactory _courseLinkFactory;
    private readonly IMediator _mediator;

    public GetCoursesQueryHandler(
        ICourseLinkFactory courseLinkFactory,
        IMediator mediator)
    {
        _courseLinkFactory = courseLinkFactory;
        _mediator = mediator;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        var innerQuery = new GetCourseSummariesQuery(request.PagedQuery);
        Result<CourseCollectionDto> innerQueryResult = await _mediator.Send(innerQuery, cancellationToken);

        if (innerQueryResult.IsFailure)
        {
            return innerQueryResult;
        }

        CourseCollectionDto dto = innerQueryResult.Value;
        CourseCollectionDto enrichedDto = dto.EnrichWithLinks(_courseLinkFactory, request.PagedQuery);

        return Result.Success(enrichedDto);
    }
}

internal static class CourseCollectionDtoEnrichmentExtensions
{
    public static CourseCollectionDto EnrichWithLinks(
        this CourseCollectionDto dto,
        ICourseLinkFactory courseLinkFactory,
        PagedQueryDto pagedQuery)
    {
        var courseDtos = dto.Items.Select(courseDto =>
        {
            var courseId = new CourseId(courseDto.Id);
            var instructorId = new UserId(courseDto.Instructor.Id);
            IReadOnlyList<LinkDto> links = courseLinkFactory.CreateLinks(
                new CourseState(courseId, instructorId, courseDto.Status));

            return courseDto with { Links = links };
        }).ToList();

        return new CourseCollectionDto
        {
            Items = courseDtos,
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalItems = dto.TotalItems,
            Links = courseLinkFactory.CreateCollectionLinks(
                pagedQuery with { Page = dto.PageNumber, PageSize = dto.PageSize },
                dto.TotalItems)
        };
    }
}

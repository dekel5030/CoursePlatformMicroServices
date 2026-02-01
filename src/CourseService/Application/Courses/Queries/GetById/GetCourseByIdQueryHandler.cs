using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetCoursePage;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CoursePageDto>
{
    private readonly ILinkBuilderService _linkBuilder;
    private readonly IMediator _mediator;
    private readonly IReadDbContext _dbContext;
    private readonly IUserContext _userContext;

    public GetCourseByIdQueryHandler(
        ILinkBuilderService linkBuilder,
        IMediator mediator,
        IReadDbContext dbContext,
        IUserContext userContext)
    {
        _linkBuilder = linkBuilder;
        _mediator = mediator;
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<CoursePageDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var innerQuery = new GetCoursePageQuery(request.Id.Value);
        Result<CoursePageDto> innerQueryResult = await _mediator.Send(innerQuery, cancellationToken);

        if (innerQueryResult.IsFailure)
        {
            return innerQueryResult;
        }

        CoursePageDto dto = innerQueryResult.Value;
        var courseId = new CourseId(dto.Id);

        UserId? currentUserId = _userContext.Id == null ? null : new UserId(_userContext.Id.Value);

        bool userHasExistingRating = currentUserId is not null &&
            await _dbContext.CourseRatings.AnyAsync(
                r => r.CourseId == courseId && r.UserId == currentUserId,
                cancellationToken);

        var ratingEligibilityContext = new CourseRatingEligibilityContext(
            courseId,
            currentUserId,
            userHasExistingRating);
        dto.Links.AddRange(_linkBuilder.BuildLinks(
            LinkResourceKey.CourseRatingEligibility,
            ratingEligibilityContext));

        dto.EnrichWithLinks(_linkBuilder);

        return Result.Success(dto);
    }
}

internal static class DtoEnrichmentExtensions
{
    public static void EnrichWithLinks(this CoursePageDto dto, ILinkBuilderService linkBuilder)
    {
        var courseContext = dto.ToCourseContext();
        dto.Links.AddRange(linkBuilder.BuildLinks(LinkResourceKey.Course, courseContext));
        foreach (ModuleDto module in dto.Modules)
        {
            var moduleContext = module.ToModuleContext(courseContext);
            module.Links.AddRange(linkBuilder.BuildLinks(LinkResourceKey.Module, moduleContext));
            foreach (LessonDto lesson in module.Lessons)
            {
                var lessonContext = lesson.ToLessonContext(
                    moduleContext,
                    false);

                lesson.Links.AddRange(linkBuilder.BuildLinks(LinkResourceKey.Lesson, lessonContext));
            }
        }
    }
}

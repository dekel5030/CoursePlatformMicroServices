using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Features.CoursePage;
using Courses.Application.Features.Shared.Loaders;
using Courses.Application.ReadModels;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.CoursePage;

internal sealed class CoursePageQueryHandler
    : IQueryHandler<CoursePageQuery, CoursePageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly ICoursePageDataLoader _coursePageDataLoader;
    private readonly ICoursePageComposer _composer;
    private readonly IImmediateEventBus _immediateEventBus;
    private readonly IUserContext _userContext;

    public CoursePageQueryHandler(
        IReadDbContext readDbContext,
        ICoursePageDataLoader coursePageDataLoader,
        ICoursePageComposer composer,
        IImmediateEventBus immediateEventBus,
        IUserContext userContext)
    {
        _readDbContext = readDbContext;
        _coursePageDataLoader = coursePageDataLoader;
        _composer = composer;
        _immediateEventBus = immediateEventBus;
        _userContext = userContext;
    }

    public async Task<Result<CoursePageDto>> Handle(
        CoursePageQuery request,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.Id);

        CoursePageData? courseData = await _coursePageDataLoader.LoadAsync(courseId, cancellationToken);

        if (courseData == null)
        {
            return Result.Failure<CoursePageDto>(CourseErrors.NotFound);
        }

        CourseAnalytics? analytics = await FetchAnalyticsAsync(courseId, cancellationToken);

        await PublishViewedEventAsync(request.Id, cancellationToken);

        CoursePageDto dto = _composer.Compose(courseData, analytics);
        return Result.Success(dto);
    }

    private async Task<CourseAnalytics?> FetchAnalyticsAsync(
        CourseId courseId,
        CancellationToken cancellationToken = default)
    {
        return await _readDbContext.CourseAnalytics
            .FirstOrDefaultAsync(c => c.CourseId == courseId.Value, cancellationToken);
    }

    private async Task PublishViewedEventAsync(
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        await _immediateEventBus.PublishAsync(
            new CourseViewedIntegrationEvent(courseId, _userContext.Id, DateTimeOffset.UtcNow),
            cancellationToken);
    }
}

using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.ReadModels;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseWithAnalyticsDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly ILinkBuilderService _linkBuilder;

    public GetCourseByIdQueryHandler(
        IReadDbContext readDbContext,
        IStorageUrlResolver urlResolver,
        ILinkBuilderService linkBuilder)
    {
        _readDbContext = readDbContext;
        _urlResolver = urlResolver;
        _linkBuilder = linkBuilder;
    }

    public async Task<Result<CourseWithAnalyticsDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _readDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseWithAnalyticsDto>(CourseErrors.NotFound);
        }

        CourseAnalytics? analytics = await _readDbContext.CourseAnalytics
            .FirstOrDefaultAsync(c => c.CourseId == request.Id.Value, cancellationToken);

        CourseContext courseContext = new(course.Id, course.InstructorId, course.Status);
        var resolvedImageUrls = course.Images
            .Select(img => _urlResolver.Resolve(StorageCategory.Public, img.Path).Value)
            .ToList();

        var courseDto = new CourseDto
        {
            Id = course.Id.Value,
            Title = course.Title.Value,
            Description = course.Description.Value,
            InstructorId = course.InstructorId.Value,
            CategoryId = course.CategoryId.Value,
            Status = course.Status,
            Price = course.Price,
            UpdatedAtUtc = course.UpdatedAtUtc,
            ImageUrls = resolvedImageUrls.AsReadOnly(),
            Tags = course.Tags.Select(t => t.Value).ToList().AsReadOnly(),
            Links = _linkBuilder.BuildLinks(LinkResourceKey.Course, courseContext).ToList()
        };

        CourseAnalyticsDto analyticsDto = analytics != null
            ? new CourseAnalyticsDto(
                analytics.EnrollmentsCount,
                analytics.TotalLessonsCount,
                analytics.TotalCourseDuration,
                analytics.AverageRating,
                analytics.ReviewsCount)
            : new CourseAnalyticsDto(0, 0, TimeSpan.Zero, 0, 0);

        return Result.Success(new CourseWithAnalyticsDto(courseDto, analyticsDto));
    }
}

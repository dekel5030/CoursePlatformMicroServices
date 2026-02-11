using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Dtos;
using Courses.Application.ReadModels;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCourseAnalytics;

internal sealed class GetCourseAnalyticsQueryHandler
    : IQueryHandler<GetCourseAnalyticsQuery, CourseDetailedAnalyticsDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IUserContext _userContext;

    public GetCourseAnalyticsQueryHandler(IReadDbContext readDbContext, IUserContext userContext)
    {
        _readDbContext = readDbContext;
        _userContext = userContext;
    }

    public async Task<Result<CourseDetailedAnalyticsDto>> Handle(
        GetCourseAnalyticsQuery request,
        CancellationToken cancellationToken = default)
    {
        if (_userContext.Id is null || !_userContext.IsAuthenticated)
        {
            return Result.Failure<CourseDetailedAnalyticsDto>(CourseErrors.Unauthorized);
        }

        var courseId = new CourseId(request.CourseId);

        Guid instructorId = await _readDbContext.Courses
            .AsNoTracking()
            .Where(c => c.Id == courseId)
            .Select(c => c.InstructorId.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (instructorId == Guid.Empty)
        {
            return Result.Failure<CourseDetailedAnalyticsDto>(CourseErrors.NotFound);
        }

        if (instructorId != _userContext.Id.Value)
        {
            return Result.Failure<CourseDetailedAnalyticsDto>(CourseErrors.Unauthorized);
        }

        CourseAnalytics? analytics = await _readDbContext.CourseAnalytics
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CourseId == request.CourseId, cancellationToken);

        if (analytics == null)
        {
            return Result.Success(new CourseDetailedAnalyticsDto
            {
                EnrollmentsCount = 0,
                AverageRating = 0,
                ReviewsCount = 0,
                ViewCount = 0,
                TotalLessonsCount = 0,
                TotalCourseDuration = TimeSpan.Zero,
                ModuleAnalytics = [],
                EnrollmentsOverTime = []
            });
        }

        var moduleAnalytics = (analytics.ModuleAnalytics ?? [])
            .Select(ma => new ModuleAnalyticsSummaryDto(
                ma.ModuleId,
                ma.LessonCount,
                ma.TotalModuleDuration))
            .ToList();

        return Result.Success(new CourseDetailedAnalyticsDto
        {
            EnrollmentsCount = analytics.EnrollmentsCount,
            AverageRating = analytics.AverageRating,
            ReviewsCount = analytics.ReviewsCount,
            ViewCount = analytics.ViewCount,
            TotalLessonsCount = analytics.TotalLessonsCount,
            TotalCourseDuration = analytics.TotalCourseDuration,
            ModuleAnalytics = moduleAnalytics,
            EnrollmentsOverTime = []
        });
    }
}

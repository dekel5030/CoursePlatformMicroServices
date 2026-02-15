using Courses.Application.Abstractions.Data;
using Courses.Application.Features.Shared;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Courses.Application.ReadModels;

namespace Courses.Application.Features.Management.GetCourseAnalytics;

internal sealed class GetCourseAnalyticsQueryHandler
    : IQueryHandler<GetCourseAnalyticsQuery, CourseDetailedAnalyticsDto>
{
    private const int EnrollmentsOverTimeDays = 30;
    private const int CourseViewersLimit = 50;

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
        var courseId = new CourseId(request.CourseId);

        Result authResult = await AuthorizeInstructorAsync(courseId, cancellationToken);
        
        if (authResult.IsFailure)
        {
            return Result.Failure<CourseDetailedAnalyticsDto>(authResult.Error);
        }

        CourseAnalytics? analytics = await _readDbContext.CourseAnalytics
            .FirstOrDefaultAsync(c => c.CourseId == request.CourseId, cancellationToken);

        if (analytics == null)
        {
            return EmptyResult();
        }

        List<EnrollmentCountByDayDto> enrollmentsOverTime = 
            await GetEnrollmentsOverTimeAsync(courseId, cancellationToken);

        List<CourseViewerDto> courseViewers = await GetRecentUniqueViewersAsync(courseId, cancellationToken);

        return Result.Success(MapToDetailedDto(analytics, enrollmentsOverTime, courseViewers));
    }


    private async Task<Result> AuthorizeInstructorAsync(
        CourseId courseId, 
        CancellationToken cancellationToken = default)
    {
        UserId? instructorId = await _readDbContext.Courses
            .Where(course => course.Id == courseId)
            .Select(course => course.InstructorId)
            .FirstOrDefaultAsync(cancellationToken);

        if (instructorId == null)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

        return InstructorAuthorization.EnsureInstructorAuthorized(_userContext, instructorId);
    }

    private async Task<List<EnrollmentCountByDayDto>> GetEnrollmentsOverTimeAsync(
        CourseId courseId, 
        CancellationToken cancellationToken = default)
    {
        DateTimeOffset cutoff = DateTimeOffset.UtcNow.AddDays(-EnrollmentsOverTimeDays);

        List<DateTimeOffset> enrollmentDates = await _readDbContext.Enrollments
            .Where(enrollment => enrollment.CourseId == courseId && enrollment.EnrolledAt >= cutoff)
            .Select(enrollment => enrollment.EnrolledAt)
            .ToListAsync(cancellationToken);

        return enrollmentDates
            .GroupBy(date => date.UtcDateTime.Date)
            .Select(g => new EnrollmentCountByDayDto(new DateTimeOffset(g.Key, TimeSpan.Zero), g.Count()))
            .OrderBy(enrollmentCount => enrollmentCount.Date)
            .ToList();
    }

    private async Task<List<CourseViewerDto>> GetRecentUniqueViewersAsync(
        CourseId courseId,
        CancellationToken cancellationToken = default)
    {
        List<ViewerRawRow> rawViews = await (
            from v in _readDbContext.CourseViews
            where v.CourseId == courseId && v.UserId != null
            join u in _readDbContext.Users on v.UserId equals u.Id
            select new ViewerRawRow
            {
                UserId = v.UserId!.Value,
                ViewedAt = v.ViewedAt,
                FirstName = u.FirstName,
                LastName = u.LastName,
                AvatarUrl = u.AvatarUrl
            }
        ).ToListAsync(cancellationToken);

        return rawViews
            .GroupBy(v => v.UserId)
            .Select(g => MapToViewerDto(g.OrderByDescending(v => v.ViewedAt).First()))
            .OrderByDescending(v => v.ViewedAt)
            .Take(CourseViewersLimit)
            .ToList();
    }


    private static CourseDetailedAnalyticsDto MapToDetailedDto(
        CourseAnalytics analytics,
        List<EnrollmentCountByDayDto> enrollmentsOverTime,
        List<CourseViewerDto> courseViewers)
    {
        return new CourseDetailedAnalyticsDto
        {
            EnrollmentsCount = analytics.EnrollmentsCount,
            AverageRating = analytics.AverageRating,
            ReviewsCount = analytics.ReviewsCount,
            ViewCount = analytics.ViewCount,
            TotalLessonsCount = analytics.TotalLessonsCount,
            TotalCourseDuration = analytics.TotalCourseDuration,
            EnrollmentsOverTime = enrollmentsOverTime,
            CourseViewers = courseViewers,
            ModuleAnalytics = (analytics.ModuleAnalytics ?? [])
                .Select(ma => new ModuleAnalyticsSummaryDto(ma.ModuleId, ma.LessonCount, ma.TotalModuleDuration))
                .ToList()
        };
    }

    private static CourseViewerDto MapToViewerDto(ViewerRawRow row)
    {
        string displayName = $"{row.FirstName} {row.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(displayName))
        {
            displayName = "—";
        }

        return new CourseViewerDto(row.UserId, displayName, row.AvatarUrl, row.ViewedAt);
    }

    private static Result<CourseDetailedAnalyticsDto> EmptyResult()
    {
        return Result.Success(new CourseDetailedAnalyticsDto
        {
            AverageRating = 0,
            CourseViewers = [],
            EnrollmentsCount = 0,
            EnrollmentsOverTime = [],
            ModuleAnalytics = [],
            ReviewsCount = 0,
            TotalLessonsCount = 0,
            ViewCount = 0,
            TotalCourseDuration = TimeSpan.Zero
        });
    }

    private sealed class ViewerRawRow
    {
        public Guid UserId { get; init; }
        public DateTimeOffset ViewedAt { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? AvatarUrl { get; init; }
    }
}

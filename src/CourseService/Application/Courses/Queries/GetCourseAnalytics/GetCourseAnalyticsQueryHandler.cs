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
                EnrollmentsOverTime = [],
                CourseViewers = []
            });
        }

        var moduleAnalytics = (analytics.ModuleAnalytics ?? [])
            .Select(ma => new ModuleAnalyticsSummaryDto(
                ma.ModuleId,
                ma.LessonCount,
                ma.TotalModuleDuration))
            .ToList();

        List<EnrollmentCountByDayDto> enrollmentsOverTime = await GetEnrollmentsOverTimeAsync(courseId, cancellationToken);
        List<CourseViewerDto> courseViewers = await GetCourseViewersAsync(courseId, cancellationToken);

        return Result.Success(new CourseDetailedAnalyticsDto
        {
            EnrollmentsCount = analytics.EnrollmentsCount,
            AverageRating = analytics.AverageRating,
            ReviewsCount = analytics.ReviewsCount,
            ViewCount = analytics.ViewCount,
            TotalLessonsCount = analytics.TotalLessonsCount,
            TotalCourseDuration = analytics.TotalCourseDuration,
            ModuleAnalytics = moduleAnalytics,
            EnrollmentsOverTime = enrollmentsOverTime,
            CourseViewers = courseViewers
        });
    }

    private async Task<List<EnrollmentCountByDayDto>> GetEnrollmentsOverTimeAsync(
        CourseId courseId,
        CancellationToken cancellationToken)
    {
        DateTimeOffset cutoff = DateTimeOffset.UtcNow.AddDays(-EnrollmentsOverTimeDays);
        List<DateTimeOffset> enrollmentDates = await _readDbContext.Enrollments
            .Where(e => e.CourseId == courseId && e.EnrolledAt >= cutoff)
            .Select(e => e.EnrolledAt)
            .ToListAsync(cancellationToken);

        return enrollmentDates
            .GroupBy(d => d.UtcDateTime.Date)
            .Select(g => new EnrollmentCountByDayDto(new DateTimeOffset(g.Key, TimeSpan.Zero), g.Count()))
            .OrderBy(x => x.Date)
            .ToList();
    }

    private async Task<List<CourseViewerDto>> GetCourseViewersAsync(
        CourseId courseId,
        CancellationToken cancellationToken)
    {
        var rawViewerRows = await (
            from v in _readDbContext.CourseViews
            where v.CourseId == courseId && v.UserId != null
            join u in _readDbContext.Users on v.UserId equals u.Id
            select new { v.UserId, v.ViewedAt, u.FirstName, u.LastName, u.AvatarUrl }
        ).ToListAsync(cancellationToken);

#pragma warning disable IDE0037 // Member name can be simplified
        var viewerRows = rawViewerRows
            .Select(x => (UserId: x.UserId!.Value, ViewedAt: x.ViewedAt, FirstName: x.FirstName, LastName: x.LastName, AvatarUrl: (string?)x.AvatarUrl))
            .ToList();
#pragma warning restore IDE0037

        return viewerRows
            .GroupBy(x => x.UserId)
            .Select(g =>
            {
                (Guid UserId, DateTimeOffset ViewedAt, string FirstName, string LastName, string? AvatarUrl) first = g.OrderByDescending(x => x.ViewedAt).First();
                System.Collections.Generic.IEnumerable<string> parts = new[] { first.FirstName, first.LastName }.Where(s => !string.IsNullOrWhiteSpace(s));
                string displayName = string.Join(" ", parts).Trim();
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = "—";
                }

                return new CourseViewerDto(first.UserId, displayName, first.AvatarUrl, first.ViewedAt);
            })
            .OrderByDescending(x => x.ViewedAt)
            .Take(CourseViewersLimit)
            .ToList();
    }
}

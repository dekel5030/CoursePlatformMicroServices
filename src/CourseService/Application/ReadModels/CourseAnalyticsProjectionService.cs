using Courses.Application.Abstractions.Data;
using Courses.Domain.Courses.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.ReadModels;

internal sealed class CourseAnalyticsProjectionService : ICourseAnalyticsProjection
{
    private readonly IWriteDbContext _db;

    public CourseAnalyticsProjectionService(IWriteDbContext db)
    {
        _db = db;
    }

    public async Task RecalculateAsync(CourseId courseId, CancellationToken cancellationToken = default)
    {
        Guid courseIdValue = courseId.Value;

        List<Domain.Modules.Module> modules = await _db.Modules
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.Index)
            .ToListAsync(cancellationToken);

        List<Domain.Lessons.Lesson> lessons = await _db.Lessons
            .Where(l => l.CourseId == courseId)
            .ToListAsync(cancellationToken);

        int enrollmentCount = await _db.Enrollments
            .CountAsync(e => e.CourseId == courseId, cancellationToken);

        int viewCount = await _db.CourseViews
            .CountAsync(v => v.CourseId == courseId, cancellationToken);

        List<int> ratings = await _db.CourseRatings
            .Where(r => r.CourseId == courseId)
            .Select(r => r.Score)
            .ToListAsync(cancellationToken);

        int totalLessonsCount = lessons.Count;
        var totalCourseDuration = TimeSpan.FromTicks(lessons.Sum(l => l.Duration.Ticks));
        int reviewsCount = ratings.Count;
        double averageRating = reviewsCount > 0 ? ratings.Average(s => s) : 0d;

        var moduleAnalytics = modules.Select(module =>
        {
            var moduleLessons = lessons.Where(l => l.ModuleId == module.Id).ToList();
            return new ModuleAnalytics
            {
                ModuleId = module.Id.Value,
                LessonCount = moduleLessons.Count,
                TotalModuleDuration = TimeSpan.FromTicks(moduleLessons.Sum(l => l.Duration.Ticks))
            };
        }).ToList();

        CourseAnalytics? existing = await _db.CourseAnalytics
            .Include(c => c.ModuleAnalytics)
            .FirstOrDefaultAsync(c => c.CourseId == courseIdValue, cancellationToken);

        if (existing != null)
        {
            existing.TotalLessonsCount = totalLessonsCount;
            existing.TotalCourseDuration = totalCourseDuration;
            existing.AverageRating = averageRating;
            existing.ReviewsCount = reviewsCount;
            existing.EnrollmentsCount = enrollmentCount;
            existing.ViewCount = viewCount;
            existing.ModuleAnalytics.Clear();
            foreach (ModuleAnalytics ma in moduleAnalytics)
            {
                existing.ModuleAnalytics.Add(ma);
            }
        }
        else
        {
            _db.CourseAnalytics.Add(new CourseAnalytics
            {
                CourseId = courseIdValue,
                TotalLessonsCount = totalLessonsCount,
                TotalCourseDuration = totalCourseDuration,
                AverageRating = averageRating,
                ReviewsCount = reviewsCount,
                EnrollmentsCount = enrollmentCount,
                ViewCount = viewCount,
                ModuleAnalytics = moduleAnalytics
            });
        }
    }
}

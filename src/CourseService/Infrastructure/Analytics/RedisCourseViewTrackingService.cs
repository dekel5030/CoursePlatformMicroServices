using Courses.Application.Abstractions.Analytics;
using Courses.Domain.Courses.Primitives;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Courses.Infrastructure.Analytics;

internal sealed class RedisCourseViewTrackingService : ICourseViewTrackingService
{
    private const string KeyPrefix = "course:views:";
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCourseViewTrackingService> _logger;

    public RedisCourseViewTrackingService(
        IDistributedCache cache,
        ILogger<RedisCourseViewTrackingService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task TrackViewAsync(CourseId courseId, CancellationToken cancellationToken = default)
    {
        string key = GetKey(courseId);

        try
        {
            string? currentValue = await _cache.GetStringAsync(key, cancellationToken);

            long newValue = (long.TryParse(currentValue, out long parsed) ? parsed : 0) + 1;

            await _cache.SetStringAsync(
                key,
                newValue.ToString(),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                },
                cancellationToken);

            _logger.LogDebug("Tracked view for course {CourseId}. New count: {Count}", courseId, newValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to track view for course {CourseId}", courseId);
        }
    }

    public async Task<long> GetPendingViewsAsync(CourseId courseId, CancellationToken cancellationToken = default)
    {
        string key = GetKey(courseId);

        try
        {
            string? value = await _cache.GetStringAsync(key, cancellationToken);
            return long.TryParse(value, out long parsed) ? parsed : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get pending views for course {CourseId}", courseId);
            return 0;
        }
    }

    public Task<Dictionary<CourseId, long>> GetAllPendingViewsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("GetAllPendingViewsAsync is not implemented with IDistributedCache. Returning empty dictionary.");
        return Task.FromResult(new Dictionary<CourseId, long>());
    }

    public async Task ClearPendingViewsAsync(CourseId courseId, CancellationToken cancellationToken = default)
    {
        string key = GetKey(courseId);

        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Cleared pending views for course {CourseId}", courseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear pending views for course {CourseId}", courseId);
        }
    }

    private static string GetKey(CourseId courseId) => $"{KeyPrefix}{courseId.Value}";
}

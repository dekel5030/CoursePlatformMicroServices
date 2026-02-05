using Courses.Application.Abstractions.Analytics;
using Courses.Domain.Courses.Primitives;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Courses.Infrastructure.Analytics;

internal sealed class RedisCourseViewTrackingService : ICourseViewTrackingService
{
    private const string KeyPrefix = "course:views:";
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCourseViewTrackingService> _logger;

    public RedisCourseViewTrackingService(
        IConnectionMultiplexer redis,
        ILogger<RedisCourseViewTrackingService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task TrackViewAsync(CourseId courseId, CancellationToken cancellationToken = default)
    {
        string key = GetKey(courseId);

        try
        {
            IDatabase db = _redis.GetDatabase();
            
            string luaScript = @"
                local current = redis.call('INCR', KEYS[1])
                if current == 1 then
                    redis.call('EXPIRE', KEYS[1], ARGV[1])
                end
                return current";
            
            await db.ScriptEvaluateAsync(
                luaScript, 
                [key], 
                [86400]);
            
            _logger.LogDebug("Tracked view for course {CourseId}", courseId);
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
            IDatabase db = _redis.GetDatabase();
            RedisValue value = await db.StringGetAsync(key);
            return (long)(value.HasValue ? value : 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get pending views for course {CourseId}", courseId);
            return 0;
        }
    }

    public async Task<Dictionary<CourseId, long>> GetAllPendingViewsAsync(CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<CourseId, long>();

        try
        {
            IDatabase db = _redis.GetDatabase();
            IServer server = _redis.GetServers().FirstOrDefault() ?? throw new InvalidOperationException("No Redis server available");

            await foreach (RedisKey key in server.KeysAsync(database: db.Database, pattern: $"{KeyPrefix}*", pageSize: 250))
            {
                RedisValue value = await db.StringGetAsync(key);

                if (value.HasValue && long.TryParse((string?)value, out long viewCount))
                {
                    string keyString = key.ToString();
                    string courseIdString = keyString.Replace(KeyPrefix, string.Empty, StringComparison.Ordinal);

                    if (Guid.TryParse(courseIdString, out Guid courseGuid))
                    {
                        result[new CourseId(courseGuid)] = viewCount;
                    }
                }
            }

            _logger.LogDebug("Retrieved {Count} courses with pending views", result.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all pending views");
        }

        return result;
    }

    public async Task ClearPendingViewsAsync(CourseId courseId, CancellationToken cancellationToken = default)
    {
        string key = GetKey(courseId);

        try
        {
            IDatabase db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
            _logger.LogDebug("Cleared pending views for course {CourseId}", courseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear pending views for course {CourseId}", courseId);
        }
    }

    private static string GetKey(CourseId courseId) => $"{KeyPrefix}{courseId.Value}";
}

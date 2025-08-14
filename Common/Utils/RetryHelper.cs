using Microsoft.Extensions.Logging;

namespace Common.Utils;

public static class RetryHelper
{
    public static async Task RetryAsync(Func<Task> action, int maxAttempts = 3, int initialDelay = 1000, ILogger? logger = null)
    {
        await RetryAsync(async () =>
        {
            await action();
            return true;
        }, maxAttempts, initialDelay, logger);
    }

    public static async Task<T> RetryAsync<T>(Func<Task<T>> func, int maxAttempts = 3, int initialDelay = 1000, ILogger? logger = null)
    {
        if (maxAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxAttempts), "Max attempts must be greater than zero.");
        }

        if (initialDelay < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialDelay), "Initial delay must be non-negative.");
        }

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Retry attempt {Attempt} failed.", attempt + 1);
                if (attempt == maxAttempts - 1)
                {
                    throw;
                }

                await Task.Delay(initialDelay);
                initialDelay *= 2;
            }
        }

        throw new InvalidOperationException("This should never be reached.");
    }
}
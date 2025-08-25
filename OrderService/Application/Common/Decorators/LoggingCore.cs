using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Common.Decorators;

public static class LogCore
{
    public static Task Run(
        ILogger logger,
        string name,
        Func<CancellationToken, Task> action,
        LogScope? scope = null,
        CancellationToken ct = default)
        => Execute(
            logger, name, scope,
            async c => { await action(c).ConfigureAwait(false); return Unit.Value; },
            (log, n, _ /*Unit*/, ms) => log.LogInformation("Handled {Name} in {ElapsedMs} ms", n, ms),
            ct);

    public static Task<T> Run<T>(
        ILogger logger,
        string name,
        Func<CancellationToken, Task<T>> action,
        LogScope? scope = null,
        Action<ILogger, string, T, long>? onResult = null,
        CancellationToken cancellationToken = default)
        => Execute(
            logger, name, scope, action,
            onResult ?? ((log, n, _res, ms) => log.LogInformation("Handled {Name} in {ElapsedMs} ms", n, ms)),
            cancellationToken);

    private static async Task<T> Execute<T>(
        ILogger logger,
        string name,
        LogScope? scope,
        Func<CancellationToken, Task<T>> action,
        Action<ILogger, string, T, long> onResult,
        CancellationToken ct)
    {
        using var _ = scope is null ? null : logger.BeginScope(scope);
        logger.LogInformation("Handling {Name}", name);

        var sw = Stopwatch.StartNew();
        try
        {
            var result = await action(ct).ConfigureAwait(false);
            sw.Stop();
            onResult(logger, name, result, sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "Unhandled exception in {Name} after {ElapsedMs} ms", name, sw.ElapsedMilliseconds);
            throw;
        }
    }

    public sealed record LogScope(
        string? CorrelationId = null,
        string? UserId = null,
        string? MessageType = null,
        string? MessageName = null);

    private readonly struct Unit { public static readonly Unit Value = new(); }
}

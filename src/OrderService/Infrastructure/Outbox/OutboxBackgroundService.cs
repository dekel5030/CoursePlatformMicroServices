using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Outbox;

internal class OutboxBackgroundService: BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxBackgroundService> _logger;

    private const int _processorFrequencyInSeconds = 10;

    public OutboxBackgroundService(
        IServiceScopeFactory serviceScopeFactory, 
        ILogger<OutboxBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting OutboxBackgroundService...");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();

                var processed = await processor.Execute(stoppingToken);
                _logger.LogInformation("Processed {Count} outbox messages", processed);

                await Task.Delay(TimeSpan.FromSeconds(_processorFrequencyInSeconds), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("OutboxBackgroundService cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in OutboxBackgroundService");
        }
        finally
        {
            _logger.LogInformation("OutboxBackgroundService finished");
        }
    }
}
using Application.Abstractions.Messaging;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Outbox;

internal sealed class OutboxProcessor
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly ILogger<OutboxProcessor> _logger;
    private const int _batchSize = 10;

    public OutboxProcessor(ApplicationDbContext dbContext, IPublisher publisher, ILogger<OutboxProcessor> logger)
    {
        _dbContext = dbContext;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<int> Execute(CancellationToken cancellationToken = default)
    {
        var outboxMessages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.OccurredAt)
            .Take(_batchSize)
            .ToListAsync(cancellationToken);

        foreach (OutboxMessage outboxMessage in outboxMessages)
        {
            outboxMessage.ProcessedAt = DateTimeOffset.UtcNow;

            try
            {
                var messageType = Type.GetType(outboxMessage.Type, throwOnError: true)!;
                var DeserializedMsg = JsonSerializer.Deserialize(outboxMessage.Content, messageType);

                await _publisher.Publish(DeserializedMsg);

            }
            catch (Exception ex)
            {
                outboxMessage.Error = ex.Message;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return outboxMessages.Count;
    }
}


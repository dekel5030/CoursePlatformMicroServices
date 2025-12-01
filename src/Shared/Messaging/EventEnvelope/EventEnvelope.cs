namespace Messaging.EventEnvelope;

public sealed record EventEnvelope<T>
(
    string EventId,
    string? CorrelationId,
    string Producer,
    string ContractVersion,
    DateTimeOffset OccurredAtUtc,
    string AggregateId,
    long AggregateVersion,
    T Payload
)
{
    public static EventEnvelope<T> Create(
        string producer,
        string aggregateId,
        long aggregateVersion,
        T payload,
        string contractVersion,
        string? correlationId = null,
        DateTimeOffset? occurredAtUtc = null,
        string? eventId = null)
    {
        return new EventEnvelope<T>(
            EventId: eventId ?? Guid.NewGuid().ToString("N"),
            CorrelationId: correlationId ?? Guid.NewGuid().ToString("N"),
            Producer: producer,
            ContractVersion: contractVersion,
            OccurredAtUtc: occurredAtUtc ?? DateTimeOffset.UtcNow,
            AggregateId: aggregateId,
            AggregateVersion: aggregateVersion,
            Payload: payload
        );
    }
};
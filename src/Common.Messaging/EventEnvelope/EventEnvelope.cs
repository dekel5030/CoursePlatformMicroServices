namespace Common.Messaging.EventEnvelope;

public sealed record EventEnvelope<T>
(
    Guid EventId,
    string EventType,
    int Version,
    DateTimeOffset OccurredAtUtc,
    string Source,
    string? CorrelationId,
    T Payload
);
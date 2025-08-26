namespace Common.Messaging.EventEnvelope;

public interface IVersionedEntity
{
    long AggregateVersion { get; set; }
    DateTimeOffset UpdatedAtUtc { get; set; }
}
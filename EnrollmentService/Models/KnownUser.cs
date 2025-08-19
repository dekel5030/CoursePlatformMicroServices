using Common.Messaging.EventEnvelope;

namespace EnrollmentService.Models;

public class KnownUser : IVersionedEntity
{
    public int UserId { get; set; }
    public bool IsActive { get; set; }  
    public long AggregateVersion { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

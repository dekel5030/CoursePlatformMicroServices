using Common.Messaging.EventEnvelope;

namespace EnrollmentService.Models;

public class KnownCourse : IVersionedEntity
{
    public int CourseId { get; set; }
    public bool IsAvailable { get; set; }
    public long AggregateVersion { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

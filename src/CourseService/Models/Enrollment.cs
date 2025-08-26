using Common.Messaging.EventEnvelope;

namespace CourseService.Models;

public class Enrollment : IVersionedEntity
{
    public int EnrollmentId { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public bool IsActive { get; set; }
    public long AggregateVersion { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

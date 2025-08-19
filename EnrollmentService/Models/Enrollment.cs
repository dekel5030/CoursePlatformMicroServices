using System.ComponentModel.DataAnnotations;
using Common.Messaging.EventEnvelope;

namespace EnrollmentService.Models;

public class Enrollment : IVersionedEntity
{
    [Key]
    public int Id { get; set; }

    public int CourseId { get; set; }
    public int UserId { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;

    public DateTime EnrolledAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? ExpiresAt { get; set; }
    public long AggregateVersion { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
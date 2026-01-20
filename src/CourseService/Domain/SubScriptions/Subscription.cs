using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Domain.SubScriptions;

public class Subscription : Entity
{
    public Guid Id { get; private set; }
    public UserId StudentId { get; private set; }
    public DateTimeOffset StartsAtUtc { get; private set; }
    public DateTimeOffset EndsAtUtc { get; private set; }

#pragma warning disable CS8618
    private Subscription() { }

    public Subscription(Guid id, UserId studentId, DateTimeOffset startsAtUtc, DateTimeOffset endsAtUtc)
    {
        Id = id;
        StudentId = studentId;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
    }
#pragma warning restore CS8618

    public bool IsActive(DateTimeOffset now) => now >= StartsAtUtc && now <= EndsAtUtc;
}

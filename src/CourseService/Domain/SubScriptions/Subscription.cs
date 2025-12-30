using Courses.Domain.Courses;
using Kernel;

namespace Courses.Domain.SubScriptions;

public class Subscription : Entity
{
    public Guid Id { get; private set; }
    public StudentId StudentId { get; private set; }
    public DateTimeOffset StartsAtUtc { get; private set; }
    public DateTimeOffset EndsAtUtc { get; private set; }

    #pragma warning disable CS8618 
    private Subscription() { }
    #pragma warning restore CS8618 

    public bool IsActive(DateTimeOffset now) => now >= StartsAtUtc && now <= EndsAtUtc;
}
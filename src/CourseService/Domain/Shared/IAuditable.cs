namespace Courses.Domain.Shared;

public interface IAuditable
{
    DateTimeOffset CreatedAtUtc { get; }
    DateTimeOffset? UpdatedAtUtc { get; }
}

namespace Courses.Domain.Shared;

public interface IAuditable
{
    DateTimeOffset CreatedAtUtc { get; set; }
    DateTimeOffset? UpdatedAtUtc { get; set; }
}

namespace Courses.Domain.Abstractions;

public interface ITimeProvider
{
    DateTimeOffset UtcNow { get; }
}

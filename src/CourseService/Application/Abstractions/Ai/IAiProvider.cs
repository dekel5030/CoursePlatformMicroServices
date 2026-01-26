namespace Courses.Application.Abstractions.Ai;

public interface IAiProvider<T>
{
    Task<T> SendAsync(string prompt, CancellationToken cancellationToken = default);
}

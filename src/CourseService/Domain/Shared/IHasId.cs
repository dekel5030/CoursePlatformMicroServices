namespace Courses.Domain.Shared;

public interface IHasId<out TId>
{
    TId Id { get; }
}

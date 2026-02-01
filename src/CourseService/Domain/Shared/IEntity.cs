using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Shared;

public interface IEntity
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}

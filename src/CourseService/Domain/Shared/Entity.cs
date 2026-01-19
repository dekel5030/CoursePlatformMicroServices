using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Shared;

public abstract class Entity<TId> : IHasId<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public abstract TId Id { get; protected set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}

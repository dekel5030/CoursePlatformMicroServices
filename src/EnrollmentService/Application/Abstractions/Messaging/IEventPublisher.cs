namespace Application.Abstractions.Messaging;

public interface IEventPublisher
{
    Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : notnull;
}
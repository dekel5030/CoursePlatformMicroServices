namespace Application.Abstractions.Messaging;

public interface IPublisher
{
    Task Publish<TMessage>(TMessage message, CancellationToken cancellationToken = default);
}


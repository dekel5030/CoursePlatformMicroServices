using Application.Abstractions.Messaging;

namespace Infrastructure.Publishers;

public class ConsolePublisher : IPublisher
{
    public Task Publish<TMessage>(TMessage message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"---> Message {message} published");
        return Task.CompletedTask;
    }
}
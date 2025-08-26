using Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Common.Decorators;

public sealed class LoggingCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _inner;
    private readonly ILogger<LoggingCommandHandler<TCommand>> _logger;

    public LoggingCommandHandler(
        ICommandHandler<TCommand> inner, 
        ILogger<LoggingCommandHandler<TCommand>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var name = typeof(TCommand).Name;

        return LogCore.Run(
            logger: _logger,
            name: name,
            action: cancellationToken => _inner.Handle(command, cancellationToken),
            scope: new LogCore.LogScope(
                MessageType: "Command",
                MessageName: name
            ),
            cancellationToken: cancellationToken
        );
    }
}


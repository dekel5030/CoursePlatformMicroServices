using Kernel;

namespace Application.Abstractions.Messaging;

public interface ICommandHandler<TCommand> : IHandler<TCommand, Result>
    where TCommand : ICommand
{ }

public interface ICommandHandler<TCommand, TResponse> : IHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{ }

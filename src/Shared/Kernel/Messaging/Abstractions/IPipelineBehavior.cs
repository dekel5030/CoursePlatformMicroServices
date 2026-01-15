namespace Kernel.Messaging.Abstractions;

public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> nextHandler,
        CancellationToken cancellationToken = default);
}

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
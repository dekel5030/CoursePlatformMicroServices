using Application.Abstractions.Messaging;

namespace Application.Mediator;

public interface IMediator
{
    Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request, 
        CancellationToken cancellationToken = default);
}
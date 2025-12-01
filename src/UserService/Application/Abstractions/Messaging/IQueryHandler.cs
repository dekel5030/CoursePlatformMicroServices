using Kernel;

namespace Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{ }
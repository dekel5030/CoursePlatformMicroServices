using Kernel;

namespace Kernel.Messaging.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;

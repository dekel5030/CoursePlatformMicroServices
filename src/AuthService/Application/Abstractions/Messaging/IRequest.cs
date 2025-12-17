namespace Application.Abstractions.Messaging;

public interface IRequest<out TResponse> : IBaseRequest { }

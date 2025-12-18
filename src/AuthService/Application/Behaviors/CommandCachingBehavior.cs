//using Application.Abstractions.Caching;
//using Application.Abstractions.Pipeline;
//using Kernel;

//namespace Application.Behaviors;

//internal class CommandCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : ICacheableCommand<TResponse>
//    where TResponse : Result
//{
//    public Task<TResponse> Handle(
//        TRequest request, 
//        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        throw new NotImplementedException();
//    }
//}

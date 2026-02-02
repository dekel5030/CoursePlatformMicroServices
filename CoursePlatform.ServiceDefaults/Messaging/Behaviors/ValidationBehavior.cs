using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace CoursePlatform.ServiceDefaults.Messaging.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> nextHandler,
        CancellationToken cancellationToken = default)
    {
        if (!_validators.Any())
        {
            return await nextHandler();
        }

        var context = new ValidationContext<TRequest>(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => Error.Validation(f.ErrorCode, f.ErrorMessage))
            .Distinct()
            .ToList();

        if (errors.Count != 0)
        {
            return CreateValidationResult(errors);
        }

        return await nextHandler();
    }

    private static TResponse CreateValidationResult(List<Error> errors)
    {
        var validationError = new ValidationError(errors);

        if (typeof(TResponse) == typeof(Result))
        {
            return (Result.Failure(validationError) as TResponse)!;
        }

        Type resultValueType = typeof(TResponse).GetGenericArguments()[0];
        MethodInfo failureMethod = typeof(Result)
            .GetMethods()
            .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethod)
            .MakeGenericMethod(resultValueType);

        object? result = failureMethod.Invoke(null, new object[] { validationError });

        return (TResponse)result!;
    }
}

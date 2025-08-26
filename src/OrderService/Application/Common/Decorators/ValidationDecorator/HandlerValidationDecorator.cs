using Application.Abstractions.Messaging;
using FluentValidation;
using Kernel;
using SharedKernel;

namespace Application.Common.Decorators.ValidationDecorator;

public sealed class HandlerValidationDecorator<TRequest, TResponse>
    : IHandler<TRequest, Result<TResponse>>
{
    private readonly IHandler<TRequest, Result<TResponse>> _inner;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public HandlerValidationDecorator(
        IHandler<TRequest, Result<TResponse>> inner,
        IEnumerable<IValidator<TRequest>> validators)
    {
        _inner = inner;
        _validators = validators;
    }

    public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var ctx = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(ctx, cancellationToken)));
            var failures = results.SelectMany(r => r.Errors).Where(e => e is not null).ToArray();

            if (failures.Length > 0)
            {
                var errors = failures.Select(f => Error.Validation(f.PropertyName, f.ErrorMessage)).ToArray();
                ValidationError validationError = new(errors);
                return Result<TResponse>.Failure(validationError);
            }
        }

        return await _inner.Handle(request, cancellationToken);
    }
}

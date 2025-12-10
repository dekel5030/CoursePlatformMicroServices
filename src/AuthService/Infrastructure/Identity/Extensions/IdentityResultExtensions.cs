using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Extensions;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult identityResult)
    {
        if (identityResult.Succeeded)
        {
            return Result.Success();
        }

        return Result.Failure(MapErrors(identityResult.Errors));
    }

    public static Result<T> ToApplicationResult<T>(this IdentityResult identityResult)
    {
        if (identityResult.Succeeded)
        {
            throw new InvalidOperationException("Cannot convert successful IdentityResult to Result<T> without a value.");
        }

        return Result<T>.Failure(MapErrors(identityResult.Errors));
    }

    private static Error MapErrors(IEnumerable<IdentityError> identityErrors)
    {
        var errors = identityErrors.Select(e =>
            new Error(e.Code, e.Description, ResolveErrorType(e.Code)));

        if (errors.Count() == 1) return errors.First();
        return new ValidationError(errors);
    }

    private static ErrorType ResolveErrorType(string identityErrorCode)
    {
        return identityErrorCode switch
        {
            nameof(IdentityErrorDescriber.DuplicateUserName) => ErrorType.Conflict,
            nameof(IdentityErrorDescriber.DuplicateEmail) => ErrorType.Conflict,

            nameof(IdentityErrorDescriber.DuplicateRoleName) => ErrorType.Conflict,
            nameof(IdentityErrorDescriber.UserAlreadyInRole) => ErrorType.Conflict,
            nameof(IdentityErrorDescriber.UserAlreadyHasPassword) => ErrorType.Conflict,
            nameof(IdentityErrorDescriber.LoginAlreadyAssociated) => ErrorType.Conflict,

            nameof(IdentityErrorDescriber.DefaultError) => ErrorType.Failure,
            nameof(IdentityErrorDescriber.ConcurrencyFailure) => ErrorType.Failure,
            nameof(IdentityErrorDescriber.UserLockoutNotEnabled) => ErrorType.Failure,

            _ => ErrorType.Validation
        };
    }
}
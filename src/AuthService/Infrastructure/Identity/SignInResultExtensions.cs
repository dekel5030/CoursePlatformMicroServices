using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public static class SignInResultExtensions
{
    public static Result ToApplicationResult(this SignInResult signInResult)
    {
        if (signInResult.Succeeded)
        {
            return Result.Success();
        }
        if (signInResult.IsLockedOut)
        {
            return Result.Failure(AuthUserErrors.IsLockOut);
        }
        if (signInResult.IsNotAllowed)
        {
            return Result.Failure(AuthUserErrors.IsLockOut);
        }
        if (signInResult.RequiresTwoFactor)
        {
            return Result.Failure(AuthUserErrors.Required2FA);
        }

        return Result.Failure(AuthUserErrors.InvalidCredentials);
    }
}
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        if (result.Succeeded)
        {
            return Result.Success();
        }

        var firstError = result.Errors.First();
        return Result.Failure(new Error(firstError.Code, firstError.Description, ErrorType.Validation));
    }
}
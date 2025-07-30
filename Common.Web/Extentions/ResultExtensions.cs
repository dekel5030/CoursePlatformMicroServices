using Common.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Common.Web.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, IStringLocalizer localizer)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        var error = result.Error!;
        var message = error.IsPublic
            ? localizer[error.MessageKey]
            : localizer[Error.Unexpected.MessageKey];

        return new ObjectResult(new { error = message })
        {
            StatusCode = error.HttpStatus
        };
    }
}

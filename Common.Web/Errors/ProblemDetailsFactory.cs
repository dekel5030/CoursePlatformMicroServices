using Common.Errors;
using Common.Resources.ErrorMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Common.Web.Errors;

public class ProblemDetailsFactory
{
    private readonly IStringLocalizer _localizer;

    public ProblemDetailsFactory(IStringLocalizer<ErrorMessages> localizer)
    {
        _localizer = localizer;
    }

    public ProblemDetails Create(Error error, string? instance = null)
    {
        return new ProblemDetails
        {
            Type = $"https://example.com/errors/{error.MessageKey}",
            Title = error.MessageKey,
            Detail = _localizer[error.MessageKey],
            Status = error.HttpStatus,
            Instance = instance
        };
    }
}

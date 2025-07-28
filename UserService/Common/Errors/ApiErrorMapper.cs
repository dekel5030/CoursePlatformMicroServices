using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using UserService.Resources.ErrorMessages;

namespace UserService.Common.Errors
{
    public class ApiErrorMapper : IApiErrorMapper
    {
        private readonly IStringLocalizer<ErrorMessages> _localizer;

        public ApiErrorMapper(IStringLocalizer<ErrorMessages> localizer)
        {
            _localizer = localizer;
        }

        public IActionResult ToActionResult<T>(Result<T> result)
        {
            var code = result.ErrorCode ?? ErrorCode.Unexpected;

            var message = code.IsPublic()
                ? _localizer[code.ToString()]
                : _localizer[ErrorCode.Unexpected.ToString()];

            return new ObjectResult(new { error = message })
            {
                StatusCode = code.GetHttpStatusCode()
            };
        }
    }
}
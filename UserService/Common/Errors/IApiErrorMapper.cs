using Microsoft.AspNetCore.Mvc;

namespace UserService.Common.Errors
{
    public interface IApiErrorMapper
    {
        IActionResult ToActionResult<T>(Result<T> result);
    }
}
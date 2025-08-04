using AuthService.Dtos;
using AuthService.Services;
using Common.Resources.ErrorMessages;
using Common.Web.Errors;
using Common.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AuthService.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public AuthController(IAuthService authServie, ProblemDetailsFactory problemDetailsFactory)
    {
        _authService = authServie;
        _problemDetailsFactory = problemDetailsFactory;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        var result = await _authService.RegisterAsyncGrpc(registerRequestDto);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemDetailsFactory, HttpContext.Request.Path);
        }

        return Ok(result.Value);
    }
}
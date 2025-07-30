using AuthService.Dtos;
using AuthService.Services;
using Common.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AuthService.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IStringLocalizer _errorLocalizer;

    public AuthController(IAuthService authServie, IStringLocalizer errorLocalizer)
    {
        _authService = authServie;
        _errorLocalizer = errorLocalizer;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        var result = await _authService.RegisterAsync(registerRequestDto);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_errorLocalizer);
        }

        return Ok(result.Value);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using UserService.Common;
using UserService.Dtos;
using UserService.Resources.ErrorMessages;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<ErrorMessages> _errorLocalizer;

        public UsersController(IUserService userService, IStringLocalizer<ErrorMessages> errorLocalizer)
        {
            _userService = userService;
            _errorLocalizer = errorLocalizer;
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            Console.WriteLine($"--> Fetching user with ID: {id}");

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            var result = await _userService.CreateUserAsync(userCreateDto);

            if (!result.IsSuccess)
            {
                var code = result.ErrorCode ?? ErrorCode.Unexpected;

                string errorMessage = code.IsPublic()
                    ? _errorLocalizer[code.ToString()]
                    : _errorLocalizer[ErrorCode.Unexpected.ToString()];

                return StatusCode(code.GetHttpStatusCode(), new { error = errorMessage });
            }

            return CreatedAtAction(nameof(GetUserById), new { id = result.Value!.Id }, result.Value);
        }

    }
}
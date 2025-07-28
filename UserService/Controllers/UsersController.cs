using Microsoft.AspNetCore.Mvc;
using UserService.Common.Errors;
using UserService.Dtos;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IApiErrorMapper _errorMapper;

        public UsersController(IUserService userService, IApiErrorMapper errorMapper)
        {
            _userService = userService;
            _errorMapper = errorMapper;
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
                return _errorMapper.ToActionResult(result);
            }

            return CreatedAtAction(nameof(GetUserById), new { id = result.Value!.Id }, result.Value);
        }
    }
}
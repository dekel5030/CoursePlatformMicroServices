using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Common.Errors;
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

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById([Range(1, int.MaxValue)] int id)
        {
            Console.WriteLine($"--> Fetching user with ID: {id}");

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersByQuery([FromQuery] UserSearchDto userSearchDto)
        {
            var users = await _userService.GetUsersByQueryAsync(userSearchDto);

            return Ok(users);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById([Range(1, int.MaxValue)] int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result.IsSuccess)
            {
                return _errorMapper.ToActionResult(result);
            }

            return NoContent();
        }

        
    }
}
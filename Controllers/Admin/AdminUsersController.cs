using AuthService.Dtos.AuthUsers;
using AuthService.Services.Admin.Interfaces;
using Common.Web.Errors;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;
    private readonly ProblemDetailsFactory _problemFactory;

    public AdminUsersController(IAdminUserService adminUserService, ProblemDetailsFactory problemFactory)
    {
        _adminUserService = adminUserService;
        _problemFactory = problemFactory;
    }

    [HttpGet]
    public async Task<IActionResult> SearchUsers([FromQuery] UserSearchDto query)
    {
        var users = await _adminUserService.SearchUsersAsync(query);
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _adminUserService.GetUserByIdAsync(id);

        if (!result.IsSuccess)
        {
            return result.ToActionDetails(_problemFactory);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}/permissions")]
    public async Task<IActionResult> GetUserPermissions(int id)
    {
        var permissions = await _adminUserService.GetUserPermissionsAsync(id);

        return Ok(permissions);
    }

    [HttpPost("{id:int}/permissions")]
    public async Task<IActionResult> AssignUserPermission(int id, [FromBody] UserAssignPermissionDto assignPermissionDto)
    {
        var result = await _adminUserService.AddUserPermissionAsync(id, assignPermissionDto);

        if (!result.IsSuccess)
        {
            return result.ToActionDetails(_problemFactory);
        }

        return Ok();
    }

    [HttpDelete("{id:int}/permissions/{permissionId:int}")]
    public async Task<IActionResult> RemoveUserPermission(int id, int permissionId)
    {
        var result = await _adminUserService.RemoveUserPermissionAsync(id, permissionId);

        if (!result.IsSuccess)
        {
            return result.ToActionDetails(_problemFactory);
        }

        return NoContent();
    }

    [HttpGet("{id:int}/roles")]
    public async Task<IActionResult> GetUserRoles(int id)
    {
        var roles = await _adminUserService.GetUserRolesAsync(id);

        return Ok(roles);
    }

    [HttpPost("{id:int}/roles")]
    public async Task<IActionResult> AssignUserRole(int id, [FromBody] UserAssignRoleDto assignRoleDto)
    {
        var result = await _adminUserService.AddUserRoleAsync(id, assignRoleDto);

        if (!result.IsSuccess)
        {
            return result.ToActionDetails(_problemFactory);
        }

        return NoContent();
    }

    [HttpDelete("{id:int}/roles/{roleId:int}")]
    public async Task<IActionResult> RemoveUserRole(int id, int roleId)
    {
        var result = await _adminUserService.RemoveUserRoleAsync(id, roleId);

        if (!result.IsSuccess)
        {
            return result.ToActionDetails(_problemFactory);
        }

        return NoContent();
    }
}
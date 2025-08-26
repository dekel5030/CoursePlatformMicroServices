using AuthService.Dtos.AuthUsers;
using AuthService.Services.Admin.Interfaces;
using Common.Auth;
using Common.Auth.Attributes;
using Common.Web.Errors;
using Common.Web.Extensions;
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
    [HasPermission(PermissionType.CanReadAuthUser)]
    public async Task<IActionResult> SearchUsers([FromQuery] UserSearchDto query)
    {
        var users = await _adminUserService.SearchUsersAsync(query);
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionType.CanReadAuthUser)]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _adminUserService.GetUserByIdAsync(id);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:int}/permissions")]
    [HasPermission(PermissionType.CanReadAuthUser)]
    public async Task<IActionResult> GetUserPermissions(int id)
    {
        var permissions = await _adminUserService.GetUserPermissionsAsync(id);

        return Ok(permissions);
    }

    [HttpPost("{id:int}/permissions/{permissionId:int}")]
    [HasPermission(PermissionType.CanUpdateAuthUser)]
    public async Task<IActionResult> AssignUserPermission(int id, int permissionId)
    {
        var result = await _adminUserService.AddPermissionAsync(id, permissionId);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return Ok();
    }

    [HttpDelete("{id:int}/permissions/{permissionId:int}")]
    [HasPermission(PermissionType.CanUpdateAuthUser)]
    public async Task<IActionResult> RemoveUserPermission(int id, int permissionId)
    {
        var result = await _adminUserService.RemovePermissionAsync(id, permissionId);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return NoContent();
    }

    [HttpGet("{id:int}/roles")]
    [HasPermission(PermissionType.CanReadAuthUser)]
    public async Task<IActionResult> GetUserRoles(int id)
    {
        var roles = await _adminUserService.GetUserRolesAsync(id);

        return Ok(roles);
    }

    [HttpPost("{id:int}/roles/{roleId:int}")]
    [HasPermission(PermissionType.CanUpdateAuthUser)]
    public async Task<IActionResult> AssignUserRole(int id, int roleId)
    {
        var result = await _adminUserService.AssignRoleAsync(id, roleId);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return NoContent();
    }

    [HttpDelete("{id:int}/roles/{roleId:int}")]
    [HasPermission(PermissionType.CanUpdateAuthUser)]
    public async Task<IActionResult> RemoveUserRole(int id, int roleId)
    {
        var result = await _adminUserService.UnassignRoleAsync(id, roleId);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return NoContent();
    }
}
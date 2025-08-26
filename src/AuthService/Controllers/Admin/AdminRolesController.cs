using AuthService.Dtos.Roles;
using AuthService.Services.Admin.Interfaces;
using Common.Auth;
using Common.Auth.Attributes;
using Common.Web.Errors;
using Common.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers.Admin;

[ApiController]
[Route("api/admin/roles")]
public class AdminRolesController : ControllerBase
{
    private readonly IAdminRoleService _adminRoleService;
    private readonly ProblemDetailsFactory _problemFactory;

    public AdminRolesController(IAdminRoleService adminRoleService, ProblemDetailsFactory problemFactory) : base()
    {
        _adminRoleService = adminRoleService;
        _problemFactory = problemFactory;
    }

    [HttpGet]
    [HasPermission(PermissionType.CanReadRole)]
    public async Task<IActionResult> GetAllRoles()
    {
        var pagedResponseDto = await _adminRoleService.GetAllRolesAsync();

        return Ok(pagedResponseDto);
    }

    [HttpGet("{id}")]
    [HasPermission(PermissionType.CanReadRole)]
    public async Task<IActionResult> GetRole(int id)
    {
        var result = await _adminRoleService.GetRoleByIdAsync(id);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [HasPermission(PermissionType.CanCreateRole)]
    public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto createDto)
    {
        var result = await _adminRoleService.CreateRoleAsync(createDto);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return CreatedAtAction(nameof(GetRole), new { id = result.Value!.Id }, result.Value);
    }

    [HttpDelete("{id}")]
    [HasPermission(PermissionType.CanDeleteRole)]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var result = await _adminRoleService.DeleteRoleByIdAsync(id);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return NoContent();
    }

    [HttpGet("{id}/permissions")]
    [HasPermission(PermissionType.CanReadRole)]
    public async Task<IActionResult> GetRolePermissions(int id)
    {
        var pagedResponseDto = await _adminRoleService.GetRolePermissionsAsync(id);

        return Ok(pagedResponseDto);
    }

    [HttpPost("{id}/permissions")]
    [HasPermission(PermissionType.CanUpdateRole)]
    public async Task<IActionResult> AssignPermissions(int id, [FromBody] RoleAssignPermissionsDto permissions)
    {
        var result = await _adminRoleService.AssignPermissionsAsync(id, permissions);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return Ok();
    }

    [HttpDelete("{roleId}/permissions/{permissionId}")]
    [HasPermission(PermissionType.CanUpdateRole)]
    public async Task<IActionResult> RemovePermission(int roleId, int permissionId)
    {
        var result = await _adminRoleService.RemovePermissionAsync(roleId, permissionId);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return NoContent();
    }
}
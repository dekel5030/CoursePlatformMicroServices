using AuthService.Dtos.Permissions;
using AuthService.Services.Admin.Interfaces;
using Common.Auth;
using Common.Auth.Attributes;
using Common.Web.Errors;
using Common.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers.Admin;

[ApiController]
[Route("api/admin/permissions")]
public class AdminPermissionsController : ControllerBase
{
    private readonly IAdminPermissionService _permissionsService;
    private readonly ProblemDetailsFactory _problemFactory;

    public AdminPermissionsController(
            IAdminPermissionService permissionsService,
            ProblemDetailsFactory problemFactory) : base()
    {
        _permissionsService = permissionsService;
        _problemFactory = problemFactory;
    }

    [HttpGet]
    [HasPermission(PermissionType.CanReadPermission)]
    public async Task<IActionResult> GetPermissions([FromQuery] PermissionSearchDto searchDto)
    {
        var result = await _permissionsService.SearchPermissionsAsync(searchDto);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [HasPermission(PermissionType.CanReadPermission)]
    public async Task<IActionResult> GetPermission(int id)
    {
        var result = await _permissionsService.GetPermissionByIdAsync(id);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [HasPermission(PermissionType.CanCreatePermission)]
    public async Task<IActionResult> CreatePermission([FromBody] PermissionCreateDto createDto)
    {
        var result = await _permissionsService.CreatePermissionAsync(createDto);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return CreatedAtAction(nameof(GetPermission), new { id = result.Value!.Id }, result.Value);
    }

    [HttpDelete("{id}")]
    [HasPermission(PermissionType.CanDeletePermission)]
    public async Task<IActionResult> DeletePermission(int id)
    {
        var result = await _permissionsService.DeletePermissionAsync(id);

        if (!result.IsSuccess)
        {
            return result.ToActionResult(_problemFactory);
        }

        return NoContent();
    }
}
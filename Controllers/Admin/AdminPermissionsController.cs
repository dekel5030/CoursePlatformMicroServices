using AuthService.Dtos.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers.Admin;

[ApiController]
[Route("api/admin/permissions")]
public class AdminPermissionsController : ControllerBase
{
    public AdminPermissionsController() : base() { }

    [HttpGet]
    public IActionResult GetPermissions()
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    [HttpGet("{id}")]
    public IActionResult GetPermission(int id)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    [HttpPost]
    public IActionResult CreatePermission([FromBody] PermissionCreateDto createDto)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePermission(int id)
    {
        throw new NotImplementedException("This method is not implemented yet.");
    }
}
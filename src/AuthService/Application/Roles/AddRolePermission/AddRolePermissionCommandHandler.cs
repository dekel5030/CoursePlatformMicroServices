using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Domain.Permissions;
using Domain.Roles;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.AddRolePermission;

public class AddRolePermissionCommandHandler : ICommandHandler<AddRolePermissionCommand>
{
    private readonly IRoleManager<Role> _roleManager;

    public AddRolePermissionCommandHandler(IRoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result> Handle(
        AddRolePermissionCommand request, 
        CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

        throw new NotImplementedException();
    }
}

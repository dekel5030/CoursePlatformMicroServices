using Application.Abstractions.Data;
using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Domain.Permissions;
using Domain.Permissions.Errors;
using Domain.Roles;
using Domain.Roles.Errors;
using Kernel;
using Kernel.Auth;
using Kernel.Auth.AuthTypes;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.AddRolePermission;

public class AddRolePermissionCommandHandler : ICommandHandler<AddRolePermissionCommand>
{
    private readonly IRoleRepository<Role> _roleManager;
    private readonly IUnitOfWork _unitOfWork;

    public AddRolePermissionCommandHandler(
        IRoleRepository<Role> roleManager, 
        IUnitOfWork unitOfWork)
    {
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        AddRolePermissionCommand request, 
        CancellationToken cancellationToken = default)
    {
        if (PermissionParser.TryParseAction(request.Action, out var action) == false)
        {
            return Result.Failure(PermissionErrors.InvalidAction);
        }
        if (PermissionParser.TryParseResource(request.Resource, out var resource) == false)
        {
            return Result.Failure(PermissionErrors.InvalidResource);
        }

        ResourceId resourceId = request.ResourceId != null ? 
            ResourceId.Create(request.ResourceId) : ResourceId.Wildcard;

        var role = await _roleManager.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        RolePermission rolePermission = new(action, resource, resourceId);
        role.AddPermission(rolePermission);

        await _roleManager.UpdateRoleAsync(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

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

namespace Application.Roles.AddRolePermission;

public class AddRolePermissionCommandHandler : ICommandHandler<AddRolePermissionCommand>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly IUnitOfWork _unitOfWork;

    public AddRolePermissionCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteDbContext writeDbContext)
    {
        _unitOfWork = unitOfWork;
        _writeDbContext = writeDbContext;
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

        var role = await _writeDbContext.Roles.FindAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        RolePermission rolePermission = new(action, resource, resourceId);
        role.AddPermission(rolePermission);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

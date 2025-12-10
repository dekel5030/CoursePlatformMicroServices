using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Permissions;
using Domain.Roles;
using Domain.Roles.Errors;
using Kernel;
using Kernel.Auth.AuthTypes;

namespace Application.Roles.Commands.AddPermissionsToRole;

public class AddPermissionsToRoleCommandHandler : ICommandHandler<AddPermissionsToRoleCommand>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _wildcard = ResourceId.Wildcard.Value;

    public AddPermissionsToRoleCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteDbContext writeDbContext)
    {
        _unitOfWork = unitOfWork;
        _writeDbContext = writeDbContext;
    }

    public async Task<Result> Handle(
        AddPermissionsToRoleCommand request, 
        CancellationToken cancellationToken = default)
    {
        var role = await _writeDbContext.Roles.FindAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        var permissions = new List<Permission>();

        foreach (var permissionDto in request.Permissions)
        {
            Result<Permission> permissionParseResult = Permission.Parse(
                permissionDto.Effect, 
                permissionDto.Action, 
                permissionDto.Resource, 
                permissionDto.ResourceId ?? _wildcard);

            if (permissionParseResult.IsFailure)
            {
                return permissionParseResult;
            }

            permissions.Add(permissionParseResult.Value);
        }

        var permissionAddResult = role.AddPermissions(permissions);

        if (permissionAddResult.IsFailure)
        {
            return permissionAddResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

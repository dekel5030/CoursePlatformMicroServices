using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Permissions;
using Domain.Roles.Errors;
using Kernel;
using Kernel.Auth.AuthTypes;

namespace Application.Roles.Commands.RoleAddPermission;

public class RoleAddPermissionCommandHandler : ICommandHandler<RoleAddPermissionCommand>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _wildcard = ResourceId.Wildcard.Value;

    public RoleAddPermissionCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteDbContext writeDbContext)
    {
        _unitOfWork = unitOfWork;
        _writeDbContext = writeDbContext;
    }

    public async Task<Result> Handle(
        RoleAddPermissionCommand request, 
        CancellationToken cancellationToken = default)
    {
        var role = await _writeDbContext.Roles.FindAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        Result<Permission> permissionParseResult = Permission.Parse(
            request.Effect, 
            request.Action, 
            request.Resource, 
            request.ResourceId ?? _wildcard);

        if (permissionParseResult.IsFailure)
        {
            return permissionParseResult;
        }

        var permissionAddResult = role.AddPermission(permissionParseResult.Value);

        if (permissionAddResult.IsFailure)
        {
            return permissionAddResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

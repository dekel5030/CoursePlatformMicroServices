using Auth.Application.Abstractions.Data;
using Auth.Domain.Roles;
using Auth.Domain.Roles.Errors;
using Auth.Domain.Roles.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Application.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler
    : ICommandHandler<CreateRoleCommand, CreateRoleResponseDto>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(IWriteDbContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateRoleResponseDto>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken = default)
    {
        RoleName roleName = new(request.RoleName);

        if (_dbContext.Roles.Any(r => r.Name == roleName))
        {
            return Result.Failure<CreateRoleResponseDto>(RoleErrors.DuplicateName);
        }

        Result<Role> result = Role.Create(roleName);

        if (result.IsFailure)
        {
            return Result.Failure<CreateRoleResponseDto>(result.Error);
        }

        Role role = result.Value;

        await _dbContext.Roles.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CreateRoleResponseDto(role.Id.ToString(), role.Name.Value);
        return Result.Success(response);
    }
}
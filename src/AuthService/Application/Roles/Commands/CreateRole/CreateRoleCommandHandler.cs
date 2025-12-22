using Auth.Application.Abstractions.Data;
using Auth.Domain.Roles;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Application.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler(IWriteDbContext dbContext, IUnitOfWork unitOfWork)
        : ICommandHandler<CreateRoleCommand, CreateRoleResponseDto>
{
    public async Task<Result<CreateRoleResponseDto>> Handle(
        CreateRoleCommand request, 
        CancellationToken cancellationToken = default)
    {
        Result<Role> result = Role.Create(request.RoleName);

        if (result.IsFailure)
        {
            return Result.Failure<CreateRoleResponseDto>(result.Error);
        }

        Role role = result.Value;

        await dbContext.Roles.AddAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CreateRoleResponseDto(role.Id.ToString(), role.Name);
        return Result.Success(response);
    }
}
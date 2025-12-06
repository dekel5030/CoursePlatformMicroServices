using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Domain.Roles;
using Kernel;

namespace Application.Roles.CreateRole;

public class CreateRoleCommandHandler(IRoleManager<Role> roleManager)
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

        var response = new CreateRoleResponseDto(role.Id.ToString(), role.Name);

        Result creationResult = await roleManager.CreateAsync(role);

        if (creationResult.IsFailure)
        {
            return Result.Failure<CreateRoleResponseDto>(creationResult.Error);
        }

        return Result.Success(response);
    }
}
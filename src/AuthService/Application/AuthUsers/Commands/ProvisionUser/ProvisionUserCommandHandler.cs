using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Auth.Contracts.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Primitives;
using Domain.Permissions;
using Domain.Roles;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.ProvisionUser;

public class ProvisionUserCommandHandler : ICommandHandler<ProvisionUserCommand, UserAuthDataDto>
{
    private readonly IUserContext _userContext;
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public ProvisionUserCommandHandler(
        IUserContext userContext,
        IWriteDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserAuthDataDto>> Handle(
        ProvisionUserCommand request,
        CancellationToken cancellationToken)
    {
        IdentityProviderId externalId = new(request.IdentityUserId);
        AuthUser? user = await _dbContext.Users
            .Include(user => user.Roles)
            .FirstOrDefaultAsync(user => user.IdentityId == externalId, cancellationToken);

        if (user == null)
        {
            user = await ProvisionUserAsync(externalId);
        }

        var distinctPermissions = user.Permissions
            .Concat(user.Roles.SelectMany(r => r.Permissions))
            .Distinct()
            .Select(p => p.ToString())
            .ToList();

        var response = new UserAuthDataDto(
            user.Id.Value.ToString(),
            distinctPermissions,
            user.Roles.Select(role => role.Name).ToList());

        return Result.Success(response);
    }

    private async Task<AuthUser> ProvisionUserAsync(IdentityProviderId externalId)
    {
        Role? defaultRole = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == "user");

        var result = AuthUser.Create(
                externalId,
                new FullName(new FirstName("test"), new LastName("test")),
                new Email("test"),
                defaultRole!);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to provision user: {result.Error}");
        }

        AuthUser newUser = result.Value;
        await _dbContext.Users.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();

        return newUser;
    }
}
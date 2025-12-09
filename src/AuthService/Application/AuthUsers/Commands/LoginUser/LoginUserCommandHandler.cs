using Application.Abstractions.Data;
using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, CurrentUserDto>
{
    private readonly IWriteDbContext _dbContext;
    private readonly ISignService<AuthUser> _signInManager;

    public LoginUserCommandHandler(
        ISignService<AuthUser> signInManager, 
        IWriteDbContext dbContext)
    {
        _signInManager = signInManager;
        _dbContext = dbContext;
    }

    public async Task<Result<CurrentUserDto>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken = default)
    {
        LoginRequestDto requestDto = request.Dto;

        AuthUser? user = await _dbContext.AuthUsers
            .FirstOrDefaultAsync(u => u.Email == requestDto.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<CurrentUserDto>(AuthUserErrors.NotFound);
        }

        var signInResult = await _signInManager.PasswordSignInAsync(
            user, 
            requestDto.Password, 
            isPersistent: true, 
            lockoutOnFailure: true);

        if (signInResult.IsFailure)
        {
            return Result.Failure<CurrentUserDto>(signInResult.Error);
        }

        var loginResultDto = new CurrentUserDto(user.Id, user.Email, user.UserName);
        return Result.Success(loginResultDto);
    }
}
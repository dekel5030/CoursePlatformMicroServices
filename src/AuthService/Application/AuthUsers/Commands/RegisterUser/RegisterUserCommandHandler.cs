using Application.Abstractions.Data;
using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.Roles;
using Kernel;

namespace Application.AuthUsers.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, CurrentUserDto>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly ISignService<AuthUser> _signService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Role _defaultRole = null!;

    public RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteDbContext writeDbContext,
        ISignService<AuthUser> signService)
    {
        _unitOfWork = unitOfWork;
        _writeDbContext = writeDbContext;
        _defaultRole = _writeDbContext.Roles.FirstOrDefault(r => r.Name == "Designer")!;
        _signService = signService;
    }

    public async Task<Result<CurrentUserDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken = default)
    {
        RegisterRequestDto requestDto = request.Dto;

        Result<AuthUser> result = AuthUser.Create(
            requestDto.Email, 
            requestDto.UserName, 
            _defaultRole);

        if (result.IsFailure)
        {
            return Result<CurrentUserDto>.Failure(result.Error);
        }

        AuthUser authUser = result.Value;
        Result registerResult = await _signService.RegisterAsync(authUser, requestDto.Password);

        if (registerResult.IsFailure) 
        {
            return Result<CurrentUserDto>.Failure(registerResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var currentUserDto = new CurrentUserDto(authUser.Id, authUser.Email!, authUser.UserName);
        return Result<CurrentUserDto>.Success(currentUserDto);
    }
}
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;

namespace Application.AuthUsers.Commands.LoginUser;

public record LoginUserCommand(LoginRequestDto Dto) : ICommand<AuthTokensResult>;

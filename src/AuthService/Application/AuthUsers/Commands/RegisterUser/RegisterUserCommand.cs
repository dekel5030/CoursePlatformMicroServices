using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;

namespace Application.AuthUsers.Commands.RegisterUser;

public record RegisterUserCommand(RegisterRequestDto Dto) : ICommand<CurrentUserDto>;

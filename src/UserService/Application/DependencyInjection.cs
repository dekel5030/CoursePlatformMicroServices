using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Users.Commands.CreateUser;
using Users.Application.Users.Commands.UpdateUser;
using Users.Application.Users.Queries.Dtos;
using Users.Application.Users.Queries.GetUserByid;
using Users.Application.Users.Queries.GetUsers;

namespace Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetUserByIdQuery, UserReadDto>, GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetUsersQuery, CollectionDto<UserReadDto>>, GetUsersQueryHandler>();

        services.AddScoped<ICommandHandler<CreateUserCommand, CreatedUserRespondDto>, CreateUserCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand, UpdatedUserResponseDto>, UpdateUserCommandHandler>();

        return services;
    }
}
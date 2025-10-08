﻿using Application.Abstractions.Messaging;
using Application.Users.Queries.Dtos;
using Application.Users.Queries.GetUserByid;
using Application.Users.Queries.GetUsers;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetUserByIdQuery, UserReadDto>, GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetUsersQuery, CollectionDto<UserReadDto>>, GetUsersQueryHandler>();

        return services;
    }
}

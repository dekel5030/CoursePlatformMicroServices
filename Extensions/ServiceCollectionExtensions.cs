using Microsoft.Extensions.Options;
using UserService.Data;
using UserService.Options;
using Microsoft.EntityFrameworkCore;
using Common.Web.Errors;
using UserService.Profiles;
using UserService.Services;
using FluentValidation.AspNetCore;
using FluentValidation;
using UserService.Validators;

namespace UserService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUsersDbContext(configuration);
        services.AddUsersRepository();

        services.AddLocalization();

        services.AddScoped<ProblemDetailsFactory>();

        services.AddAutoMapper(typeof(UsersProfile));

        services.AddScoped<IUserService, Services.UserService>();

        services.AddGrpc();

        services.AddValidation();

        return services;
    }

    private static IServiceCollection AddUsersDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<UsersDbOptions>()
                .Bind(configuration.GetSection(UsersDbOptions.SectionName));

        services.AddDbContext<UsersDbContext>((sp, options) =>
        {
            var dbSettings = sp.GetRequiredService<IOptions<UsersDbOptions>>().Value;
            options.UseNpgsql(dbSettings.BuildConnectionString());
        });

        return services;
    }

    private static IServiceCollection AddUsersRepository(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UsersRepository>();

        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<UserCreateDtoValidator>();
        
        return services;
    }
}

internal interface IUsersRepository
{
}
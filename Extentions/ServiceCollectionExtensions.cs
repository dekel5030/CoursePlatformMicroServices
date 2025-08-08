using AuthService.Data.Context;
using AuthService.Data.Repositories.Implementations;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Security;
using AuthService.Services;
using AuthService.Settings;
using AuthService.SyncDataServices.Grpc;
using AuthService.SyncDataServices.Http;
using AuthService.Validators;
using Common.Rollback;
using Common.Web.Errors;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static Common.Grpc.GrpcUserService;

namespace AuthService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAppDbContext(config);
        services.AddRepositories();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IAuthService, Services.AuthService>();
        services.AddScoped<IRollbackManager, StackRollbackManager>();
        services.AddScoped<ProblemDetailsFactory>();
        services.AddJwtAuth(config);

        services.AddHttpClient<IUserServiceDataClient, HttpUserServiceDataClient>();

        services.AddGrpcClient<GrpcUserServiceClient>(s =>
        {
            s.Address = new Uri(config["Grpc:UserServiceUrl"]!);
        });
        services.AddScoped<IGrpcUserServiceDataClient, GrpcUserServiceDataClient>();
        services.AddHttpContextAccessor();

        services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<Program>();

        services.Configure<ValidationSettings>(
            config.GetSection("ValidationSettings"));

        return services;
    }

    private static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<AuthDbSettings>().Bind(config.GetSection(AuthDbSettings.SectionName))
            .ValidateDataAnnotations().ValidateOnStart();

        services.AddDbContext<AuthDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<IOptions<AuthDbSettings>>().Value;
            options.UseNpgsql(settings.BuildConnectionString());
        });

        return services;
    }

    private static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<JwtOptions>().Bind(config.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations().ValidateOnStart();

        services.AddSingleton<ITokenService, TokenService>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }
}
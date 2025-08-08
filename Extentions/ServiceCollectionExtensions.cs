using AuthService.Data.Context;
using AuthService.Data.Repositories.Implementations;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Handlers;
using AuthService.Security;
using AuthService.Services;
using AuthService.Services.Admin.Implementations;
using AuthService.Services.Admin.Interfaces;
using AuthService.Settings;
using AuthService.SyncDataServices.Grpc;
using AuthService.SyncDataServices.Http;
using AuthService.Validators;
using Common.Auth.Extentions;
using Common.Rollback;
using Common.Web.Errors;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static Common.Grpc.GrpcUserService;

namespace AuthService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthDbContext(config);
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddJwtAuthentication(config);
        services.AddJwtTokenGeneration();
        services.AddPermissionAuthorization();

        services.AddRepositories();
        services.AddAdminServices();

        services.AddScoped<IAuthService, Services.AuthService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IRollbackManager, StackRollbackManager>();
        services.AddScoped<ProblemDetailsFactory>();

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

    private static IServiceCollection AddAuthDbContext(this IServiceCollection services, IConfiguration config)
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

    private static IServiceCollection AddJwtTokenGeneration(this IServiceCollection services)
    {
        services.AddSingleton<ITokenService, TokenService>();

        return services;
    }

    private static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        return services;
    }

    private static IServiceCollection AddAdminServices(this IServiceCollection services)
    {
        services.AddScoped<IAdminPermissionService, AdminPermissionService>();

        return services;
    }
}
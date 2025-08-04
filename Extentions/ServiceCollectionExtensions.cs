using AuthService.Data;
using AuthService.Security;
using AuthService.Services;
using AuthService.SyncDataServices.Grpc;
using AuthService.SyncDataServices.Http;
using Common.Rollback;
using Common.Web.Errors;
using Microsoft.EntityFrameworkCore;
using static Common.Grpc.GrpcUserService;

namespace AuthService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<IAuthService, Services.AuthService>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IRollbackManager, StackRollbackManager>();
        services.AddScoped<ProblemDetailsFactory>();

        services.AddHttpClient<IUserServiceDataClient, HttpUserServiceDataClient>();

        services.AddGrpcClient<GrpcUserServiceClient>(s =>
        {
            s.Address = new Uri(config["Grpc:UserServiceUrl"]!);
        });
        services.AddScoped<IGrpcUserServiceDataClient, GrpcUserServiceDataClient>();
        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            Console.WriteLine("--> using development database");
            services.AddDbContext<AuthDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("AuthDb")));
        }

        return services;
    }
}

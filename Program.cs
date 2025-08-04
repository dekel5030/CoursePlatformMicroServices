using AuthService.Data;
using AuthService.Security;
using AuthService.Services;
using AuthService.SyncDataServices.Grpc;
using AuthService.SyncDataServices.Http;
using Common.Grpc;
using Common.Rollback;
using Common.Web.Errors;
using Microsoft.EntityFrameworkCore;
using static Common.Grpc.GrpcUserService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddHttpClient<IUserServiceDataClient, HttpUserServiceDataClient>();
builder.Services.AddLocalization();
builder.Services.AddScoped<ProblemDetailsFactory>();
builder.Services.AddScoped<IRollbackManager, StackRollbackManager>();

builder.Services.AddGrpcClient<GrpcUserServiceClient>(s =>
{
    s.Address = new Uri(builder.Configuration["Grpc:UserServiceUrl"]!);
});
builder.Services.AddScoped<IGrpcUserServiceDataClient, GrpcUserServiceDataClient>();


if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("--> using development database");
    builder.Services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDb")));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await PrepDb.InitializeAsync(app);
}

var supportedCultures = new[] { "en", "he" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();

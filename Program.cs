using System.Globalization;
using Common.Web.Errors;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Profiles;
using UserService.Services;
using UserService.SyncDataServices.Grpc;
using UserService.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UsersRepository>();

builder.Services.AddLocalization();
builder.Services.AddScoped<ProblemDetailsFactory>();

builder.Services.AddAutoMapper(typeof(UsersProfile));
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateDtoValidator>();

builder.Services.AddGrpc();

if (builder.Environment.IsDevelopment())
    Console.WriteLine("--> Using Development Database");
else
    Console.WriteLine("--> Using Production Database");

var connectionString = builder.Configuration.GetConnectionString("UsersDb");
builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //await PrepDb.PopulateAsync(app);
}

var supportedCultures = new[] { "en", "he" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("he")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseHttpsRedirection();
app.UseRequestLocalization(localizationOptions);

app.MapControllers();
app.MapGrpcService<GrpcUserService>();
app.Run();


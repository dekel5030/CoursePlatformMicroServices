using System.Globalization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Common.Errors;
using UserService.Data;
using UserService.Profiles;
using UserService.Services;
using UserService.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UsersRepository>();

builder.Services.AddLocalization();

builder.Services.AddAutoMapper(typeof(UsersProfile));
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateDtoValidator>();

builder.Services.AddScoped<IApiErrorMapper, ApiErrorMapper>();

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

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.MapControllers();

app.Run();


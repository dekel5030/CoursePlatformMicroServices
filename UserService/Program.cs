using System.Globalization;
using System.Resources;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UserService.Common.Errors;
using UserService.Data;
using UserService.Profiles;
using UserService.Resources;
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
{
    Console.WriteLine("--> Using Development Database");
    var connectionString = builder.Configuration.GetConnectionString("UsersDb");
    builder.Services.AddDbContext<UsersDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    Console.WriteLine("--> Using Production Database");
    builder.Services.AddDbContext<UsersDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await PrepDb.PopulateAsync(app);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var supportedCultures = new[] { "en", "he" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.MapControllers();

app.Run();


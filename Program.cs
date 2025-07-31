using System.Globalization;
using System.Linq;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Common.Errors;
using UserService.Data;
using UserService.Profiles;
using UserService.Services;
using UserService.Validators;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => char.ToLowerInvariant(kvp.Key[0]) + kvp.Key[1..], // Convert to camelCase
                    kvp => kvp.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return new BadRequestObjectResult(new { errors });
        };
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddScoped<IUserRepository, UsersRepository>();

builder.Services.AddLocalization();

builder.Services.AddAutoMapper(typeof(UsersProfile));
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateDtoValidator>();

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


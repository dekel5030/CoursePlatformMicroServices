using AuthService.Data;
using AuthService.Security;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("--> using development database");
    builder.Services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(builder.Configuration["AuthDb"]));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

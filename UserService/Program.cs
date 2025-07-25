using Microsoft.EntityFrameworkCore;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UsersRepository>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<UsersDbContext>(options =>
        options.UseInMemoryDatabase("UsersDbInMem"));
}

var app = builder.Build();
await PrepDb.PopulateAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.UseHttpsRedirection();


app.Run();


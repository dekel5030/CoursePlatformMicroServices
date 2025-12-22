using Users.Api.Endpoints;
using Users.Api.Extensions;
using Users.Infrastructure;
using Users.Infrastructure.Extensions;
using Users.Application;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructureDefaults();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpoints(typeof(IEndpoint).Assembly);

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("LocalDev");
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

app.UseInfrastructure();

app.MapEndpoints();
app.UseInfrastructureDefaultEndpoints();
app.Run();

public partial class Program { }
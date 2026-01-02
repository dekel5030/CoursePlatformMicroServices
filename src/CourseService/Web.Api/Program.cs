using Courses.Api.Endpoints;
using Courses.Api.Extensions;
using Courses.Infrastructure;
using Courses.Application;
using Courses.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructureDefaults();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddEndpoints(typeof(IEndpoint).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("LocalDev");
    app.ApplyMigrations();
}

app.UseHttpsRedirection();
app.MapGet("/", () => "OK");
app.MapEndpoints();

app.UseInfrastructureDefaultEndpoints();

app.Run();

public partial class Program { }
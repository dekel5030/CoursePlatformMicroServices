using Auth.Api.Endpoints;
using Auth.Api.Extensions;
using Auth.Application;
using Auth.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

builder.AddSwaggerGenWithAuth();
builder.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddEndpoints(typeof(IEndpoint).Assembly);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service API V1");
    });
    app.UseCors("LocalDev");
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();
app.UseInfrastructure();

app.MapEndpoints();

await app.RunAsync();

#pragma warning disable CA1515 // Consider making public types internal
#pragma warning disable ASP0027 // Unnecessary public Program class declaration
#pragma warning disable S1118 // Utility classes should not have public constructors
public partial class Program { }
#pragma warning restore S1118 // Utility classes should not have public constructors
#pragma warning restore ASP0027 // Unnecessary public Program class declaration
#pragma warning restore CA1515 // Consider making public types internal

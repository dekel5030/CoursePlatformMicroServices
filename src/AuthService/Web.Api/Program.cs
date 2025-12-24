using Auth.Api.Endpoints;
using Auth.Api.Extensions;
using Auth.Application;
using Auth.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddOpenApi();

builder.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddEndpoints(typeof(IEndpoint).Assembly);
builder.Services.AddSwaggerGenWithAuth();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service API V1");
    });
    app.MapOpenApi();
    app.UseCors("LocalDev");
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();
app.UseInfrastructure();

app.MapEndpoints();

app.Run();

public partial class Program { }

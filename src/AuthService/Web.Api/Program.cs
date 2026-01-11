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

builder.AddSwaggerGenWithAuth();
builder.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddEndpoints(typeof(IEndpoint).Assembly);

var app = builder.Build();

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
public partial class Program { }
#pragma warning restore CA1515 // Consider making public types internal

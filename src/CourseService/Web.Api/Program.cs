using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Endpoints;
using Courses.Api.Extensions;
using Courses.Application;
using Courses.Infrastructure;
using Courses.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructureDefaults();
builder.AddDefaultOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddEndpoints(typeof(IEndpoint).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapEndpoints();

app.UseInfrastructureDefaultEndpoints();
app.UseInfrastructure();

await app.RunAsync();

#pragma warning disable CA1515 // Consider making public types internal
public partial class Program { }
#pragma warning restore CA1515 // Consider making public types internal

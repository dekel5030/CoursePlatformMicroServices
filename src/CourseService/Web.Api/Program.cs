using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Endpoints;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure;
using Courses.Api.Infrastructure.JsonConverters;
using Courses.Application;
using Courses.Infrastructure;
using Courses.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructureDefaults();
builder.AddDefaultOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddEndpoints(typeof(IEndpoint).Assembly);
builder.Services.AddValueObjectConverter();
builder.Services.AddValueObjectModelBinding();

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

app.Run();

public partial class Program { }
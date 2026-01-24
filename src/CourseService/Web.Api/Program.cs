using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Endpoints;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application;
using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Infrastructure;
using Courses.Infrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All;
});


builder.AddInfrastructureDefaults();
builder.AddDefaultOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddEndpoints(typeof(IEndpoint).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICourseLinkProvider, CourseLinkProvider>();
builder.Services.AddScoped<IModuleLinkProvider, ModuleLinkProvider>();
builder.Services.AddScoped<ILessonLinkProvider, LessonLinkProvider>();

WebApplication app = builder.Build();

app.UseForwardedHeaders();

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

#pragma warning disable ASP0027 // Unnecessary public Program class declaration
#pragma warning disable CA1515 // Consider making public types internal
public partial class Program
{
    private Program()
    {

    }
}
#pragma warning restore CA1515 // Consider making public types internal
#pragma warning restore ASP0027 // Unnecessary public Program class declaration

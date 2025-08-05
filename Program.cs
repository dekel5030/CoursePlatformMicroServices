using CourseService.Data;
using CourseService.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCourseServiceDependencies(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

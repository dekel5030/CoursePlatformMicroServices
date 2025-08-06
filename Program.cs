using CourseService.Data;
using CourseService.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCourseServiceDependencies(builder.Configuration);

var app = builder.Build();

PrepDb.PrepPopulation(app);

app.MapGet("/", () => "Hello World!");
app.MapCourseEndpoints();


app.UseAuthentication();
app.UseAuthorization();

app.Run();

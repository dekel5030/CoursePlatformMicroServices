using Common.Web.Swagger;
using CourseService.Data;
using CourseService.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCourseServiceDependencies(builder.Configuration);

var app = builder.Build();

PrepDb.PrepPopulation(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapCourseEndpoints();
app.UseAppSwagger();

app.Run();

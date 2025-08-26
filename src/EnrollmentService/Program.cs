using Common.Web.Swagger;
using EnrollmentService.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEnrollmentDependencies();

var app = builder.Build();

app.UseAppDependencies();

app.MapEnrollmentEndpoints();
app.UseAppSwagger();

app.Run();

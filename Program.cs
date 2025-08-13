using EnrollmentService.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEnrollmentDependencies();

var app = builder.Build();

app.Run();

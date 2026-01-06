using CoursePlatform.ServiceDefaults;
using CoursePlatform.ServiceDefaults.Endpoints;
using StorageService.S3;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.ConfigureS3(builder.Configuration);
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapEndpoints();

app.UseHttpsRedirection();

app.Run();

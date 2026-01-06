using CoursePlatform.ServiceDefaults;
using CoursePlatform.ServiceDefaults.Endpoints;
using CoursePlatform.ServiceDefaults.Swagger;
using StorageService.Masstransit;
using StorageService.S3;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDefaultOpenApi();
builder.Services.AddMassTransitInternal(builder.Configuration);
builder.Services.ConfigureS3(builder.Configuration);
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();
app.MapEndpoints();

app.UseHttpsRedirection();

app.Run();

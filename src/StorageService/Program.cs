using CoursePlatform.ServiceDefaults;
using CoursePlatform.ServiceDefaults.Endpoints;
using CoursePlatform.ServiceDefaults.Swagger;
using StorageService.Masstransit;
using StorageService.S3;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDefaultOpenApi();
builder.Services.AddMassTransitInternal(builder.Configuration);
builder.Services.ConfigureS3(builder.Configuration);
builder.Services.AddEndpoints(typeof(Program).Assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

WebApplication app = builder.Build();
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();
app.MapEndpoints();

app.UseHttpsRedirection();

await app.RunAsync();

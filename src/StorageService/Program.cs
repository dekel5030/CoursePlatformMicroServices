using System.Diagnostics;
using CoursePlatform.ServiceDefaults;
using CoursePlatform.ServiceDefaults.Endpoints;
using CoursePlatform.ServiceDefaults.Swagger;
using StorageService.Masstransit;
using StorageService.S3;
using StorageService.Transcription;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string openAiKey = builder.Configuration["OpenAI:ApiKey"]
    ?? throw new InvalidOperationException("OpenAI:ApiKey is missing from configuration!");

builder.Services.AddOpenAiTranscriptionService(openAiKey);
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

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:5173");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "*");
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();
app.MapEndpoints();

app.UseHttpsRedirection();

await app.RunAsync();

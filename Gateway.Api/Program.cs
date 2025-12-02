using Gateway.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddGateway();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseGatway();

app.Run();

using Common.Grpc;
using UserService.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddUserServiceDependencies(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseUserServiceDependencies();
app.MapUserServiceEndpoints();

app.Run();


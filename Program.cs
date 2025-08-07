using AuthService.Data;
using AuthService.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddLocalization();
builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await PrepDb.InitializeAsync(app);
}

app.UseLocalizationSetup();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

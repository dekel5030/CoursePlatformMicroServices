using Microsoft.EntityFrameworkCore;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UsersRepository>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<UsersDbContext>(options =>
        options.UseInMemoryDatabase("UsersDbInMem"));
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await PrepDb.PopulateAsync(app);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var supportedCultures = new[] { "en", "he" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("he")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);


app.UseRequestLocalization(localizationOptions);
app.UseHttpsRedirection();
app.MapControllers();


app.Run();


using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace OrderService.IntegrationTests;

public abstract class IntegrationTestsBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private readonly RabbitMqContainer _rabbitMq;

    protected WebApplicationFactory<Program> Factory { get; private set; }= null!;
    protected HttpClient Client { get; private set; } = null!;

    protected IntegrationTestsBase()
    {
        _postgres = new PostgreSqlBuilder()
            .WithDatabase("ordersdb")
            .WithUsername("postgres")
            .WithPassword("yourpassword")
            .Build();

        _rabbitMq = new RabbitMqBuilder()
            .WithUsername("guest")
            .WithPassword("guest")
            .Build();

    }
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await _rabbitMq.StartAsync();

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    var dict = new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Database"] = _postgres.GetConnectionString(),
                        ["ConnectionStrings:RabbitMq"] = _rabbitMq.GetConnectionString(),
                    };

                    config.AddInMemoryCollection(dict);
                });
            });

        Client = Factory.CreateClient();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await _rabbitMq.DisposeAsync();
    }
}

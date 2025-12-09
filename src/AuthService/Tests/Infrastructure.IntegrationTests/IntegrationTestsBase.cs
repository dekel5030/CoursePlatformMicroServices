using DotNet.Testcontainers.Builders;
using Infrastructure.Database;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace Infrastructure.IntegrationTests;

/// <summary>
/// Base class for infrastructure integration tests.
/// Provides Testcontainers setup for PostgreSQL and RabbitMQ to test real database interactions.
/// </summary>
public abstract class IntegrationTestsBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private readonly RabbitMqContainer _rabbitMq;

    protected WebApplicationFactory<Program> Factory { get; private set; } = null!;
    protected HttpClient Client { get; private set; } = null!;
    protected IBusControl ExternalBus { get; private set; } = null!;

    protected IntegrationTestsBase()
    {
        _postgres = new PostgreSqlBuilder()
            .WithDatabase("authdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        _rabbitMq = new RabbitMqBuilder()
            .WithImage("rabbitmq:3-management")
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(5674, 5672)
            .WithPortBinding(15674, 15672)
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
                        ["ConnectionStrings:WriteDatabase"] = _postgres.GetConnectionString(),
                        ["ConnectionStrings:ReadDatabase"] = _postgres.GetConnectionString(),
                        ["ConnectionStrings:RabbitMq"] = _rabbitMq.GetConnectionString(),
                    };

                    config.AddInMemoryCollection(dict);
                });
            });

        Client = Factory.CreateClient();

        ExternalBus = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(_rabbitMq.GetConnectionString());
        });
        await ExternalBus.StartAsync();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await ExternalBus.StopAsync();
        await _postgres.DisposeAsync();
        await _rabbitMq.DisposeAsync();
        await Factory.DisposeAsync();
    }

    /// <summary>
    /// Helper method to get a fresh scope for testing
    /// </summary>
    protected IServiceScope CreateScope()
    {
        return Factory.Services.CreateScope();
    }

    /// <summary>
    /// Helper method to get WriteDbContext
    /// </summary>
    protected WriteDbContext GetWriteDbContext(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<WriteDbContext>();
    }
}

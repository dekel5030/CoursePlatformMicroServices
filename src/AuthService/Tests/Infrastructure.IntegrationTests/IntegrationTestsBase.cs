using Auth.Infrastructure.Database;
using DotNet.Testcontainers.Builders;
using Infrastructure.Database;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using Xunit;

namespace Auth.Infrastructure.IntegrationTests;

/// <summary>
/// Base class for infrastructure integration tests.
/// Provides Testcontainers setup for PostgreSQL and RabbitMQ to test real database interactions.
/// </summary>
public abstract class IntegrationTestsBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private readonly RabbitMqContainer _rabbitMq;
    private readonly RedisContainer _redis;

    protected WebApplicationFactory<Program> Factory { get; private set; } = null!;
    protected HttpClient Client { get; private set; } = null!;
    protected IBusControl ExternalBus { get; private set; } = null!;

#pragma warning disable CA1041 // Provide ObsoleteAttribute message
#pragma warning disable S1123 // "Obsolete" attributes should include explanations
#pragma warning disable S1133 // Deprecated code should be removed
    [Obsolete]
#pragma warning restore S1133 // Deprecated code should be removed
#pragma warning restore S1123 // "Obsolete" attributes should include explanations
#pragma warning restore CA1041 // Provide ObsoleteAttribute message
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
            //.WithPortBinding(5674, 5672)
            //.WithPortBinding(15674, 15672)
            .Build();

        _redis = new RedisBuilder()
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await _rabbitMq.StartAsync();
        await _redis.StartAsync();

        //Factory = new WebApplicationFactory<Program>()
        //    .WithWebHostBuilder(builder =>
        //    {
        //        builder.ConfigureAppConfiguration((context, config) =>
        //        {
        //            var dict = new Dictionary<string, string?>
        //            {
        //                ["ConnectionStrings:WriteDatabase"] = _postgres.GetConnectionString(),
        //                ["ConnectionStrings:ReadDatabase"] = _postgres.GetConnectionString(),
        //                ["ConnectionStrings:RabbitMq"] = _rabbitMq.GetConnectionString(),
        //                ["ConnectionStrings:redis"] = _redis.GetConnectionString()
        //            };

        //            config.AddInMemoryCollection(dict);
        //        });
        //    });

        //Client = Factory.CreateClient();

        ExternalBus = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(_rabbitMq.GetConnectionString());
        });
        await ExternalBus.StartAsync();

        using IServiceScope scope = Factory.Services.CreateScope();
        WriteDbContext dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await ExternalBus.StopAsync();
        await _postgres.DisposeAsync();
        await _rabbitMq.DisposeAsync();
        await Factory.DisposeAsync();
        await _redis.DisposeAsync();
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

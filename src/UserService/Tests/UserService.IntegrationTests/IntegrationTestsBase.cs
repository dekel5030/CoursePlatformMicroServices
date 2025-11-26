using DotNet.Testcontainers.Builders;
using Infrastructure.Database;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;
using Xunit.Abstractions;

namespace UserService.IntegrationTests;

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
            .WithDatabase("usersdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            //.WithPortBinding(7777, 5432)
            .Build();

        _rabbitMq = new RabbitMqBuilder()
            .WithImage("rabbitmq:3-management")
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(5673, 5672)
            .WithPortBinding(15673, 15672)
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
        PrintDebugInfo();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await _rabbitMq.DisposeAsync();
    }

    public void PrintDebugInfo()
    {
        Debug.WriteLine($"Host: {_rabbitMq.Hostname}");
        Debug.WriteLine($"Port: {_rabbitMq.GetMappedPublicPort(5672)}");
        Debug.WriteLine($"UI:   {_rabbitMq.Hostname}:{_rabbitMq.GetMappedPublicPort(15672)}");
        Debug.WriteLine($"db: {_postgres.GetConnectionString()}");
    }
}

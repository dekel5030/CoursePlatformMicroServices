using Common.Messaging.Options;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.Messaging.Extensions;

public static class MessagingExtensions
{
    public static IServiceCollection AddMassTransitRabbitMq(
        this IServiceCollection services,
        Action<IBusRegistrationConfigurator> registerConsumers,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? perServiceConfigure = null)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            registerConsumers(x);

            x.UsingRabbitMq((ctx, cfg) =>
            {
                var rabbit = ctx.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                var mt     = ctx.GetRequiredService<IOptions<MassTransitOptions>>().Value;

                cfg.Host(rabbit.CreateUri(), h =>
                {
                    h.Username(rabbit.User);
                    h.Password(rabbit.Password);
                    if (rabbit.UseSsl) h.UseSsl(_ => { });
                });

                cfg.UseMessageRetry(r => r.Incremental(
                    mt.RetryCount,
                    TimeSpan.FromSeconds(mt.RetryInitialSeconds),
                    TimeSpan.FromSeconds(mt.RetryIncrementSeconds)));

                if (mt.PrefetchCount is ushort prefetch)
                    cfg.PrefetchCount = prefetch;

                perServiceConfigure?.Invoke(ctx, cfg);

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }

    private static void ConfigureRabbitHost(IRabbitMqBusFactoryConfigurator bus, RabbitMqOptions opt)
    {
        bus.Host(opt.CreateUri(), h =>
        {
            h.Username(opt.User);
            h.Password(opt.Password);

            if (opt.UseSsl)
            {
                h.UseSsl(_ => { });
            }
        });
    }

    private static void ApplyPolicies(IBusFactoryConfigurator bus, MassTransitOptions mt)
    {
        bus.UseMessageRetry(r => r.Incremental(
            mt.RetryCount,
            TimeSpan.FromSeconds(mt.RetryInitialSeconds),
            TimeSpan.FromSeconds(mt.RetryIncrementSeconds)));
    }
}

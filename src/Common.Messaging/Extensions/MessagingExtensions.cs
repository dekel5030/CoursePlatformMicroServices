using Common.Messaging.Options;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.Messaging.Extensions;

public static class MessagingExtensions
{
    public static IServiceCollection AddMassTransitRabbitMq(
        this IServiceCollection services,
        Action<IBusRegistrationConfigurator>? registerConsumers = null)
    {
        services.AddMassTransit(reg =>
        {
            registerConsumers?.Invoke(reg);

            reg.UsingRabbitMq((context, bus) =>
            {
                var rabbitOpt = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                var massTransitOpt = context.GetRequiredService<IOptions<MassTransitOptions>>().Value;

                ConfigureRabbitHost(bus, rabbitOpt);
                ApplyPolicies(bus, massTransitOpt);

                if (massTransitOpt.PrefetchCount is ushort prefetch)
                    bus.PrefetchCount = prefetch;

                if (massTransitOpt.UseKebabCaseEndpoints)
                    bus.ConfigureEndpoints(context);
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

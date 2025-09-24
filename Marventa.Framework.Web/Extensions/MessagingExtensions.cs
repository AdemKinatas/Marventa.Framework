using MassTransit;
using Marventa.Framework.Core.Interfaces;
using Marventa.Framework.Infrastructure.Messaging.RabbitMQ;
using Marventa.Framework.Infrastructure.Messaging.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Marventa.Framework.Web.Extensions;

public static class MessagingExtensions
{
    /// <summary>
    /// Adds RabbitMQ messaging with MassTransit
    /// </summary>
    public static IServiceCollection AddMarventaRabbitMQ(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        var connectionString = configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672/";
        var serviceName = configuration["Messaging:ServiceName"] ?? "marventa-service";

        return services.AddMarventaRabbitMQ(connectionString, serviceName, assemblies);
    }

    /// <summary>
    /// Adds RabbitMQ messaging with MassTransit with explicit configuration
    /// </summary>
    public static IServiceCollection AddMarventaRabbitMQ(
        this IServiceCollection services,
        string connectionString,
        string serviceName = "marventa-service",
        params Assembly[] assemblies)
    {
        // Register message bus service
        services.AddScoped<IMessageBus, RabbitMqMessageBus>();

        // Get assemblies to scan for consumers
        var assembliesToScan = assemblies?.Any() == true
            ? assemblies
            : new[] { Assembly.GetExecutingAssembly(), Assembly.GetCallingAssembly() };

        // Configure MassTransit
        services.AddMassTransit(x =>
        {
            // Add consumers from specified assemblies
            x.AddConsumers(assembliesToScan);

            // Configure RabbitMQ
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(connectionString);

                // Configure retries
                cfg.UseMessageRetry(r =>
                {
                    r.Incremental(
                        retryLimit: 3,
                        initialInterval: TimeSpan.FromSeconds(1),
                        intervalIncrement: TimeSpan.FromSeconds(1));
                });

                // Configure delayed redelivery
                cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5)));

                // Configure circuit breaker
                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TripThreshold = 5;
                    cb.ResetInterval = TimeSpan.FromMinutes(1);
                });

                // Configure endpoints
                cfg.ConfigureEndpoints(context, new DefaultEndpointNameFormatter(serviceName, false));
            });
        });

        return services;
    }

    /// <summary>
    /// Adds Kafka messaging
    /// </summary>
    public static IServiceCollection AddMarventaKafka(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<KafkaOptions>(options =>
            configuration.GetSection(KafkaOptions.SectionName).Bind(options));

        // Register message bus service
        services.AddSingleton<IMessageBus, KafkaMessageBus>();

        return services;
    }

    /// <summary>
    /// Adds Kafka messaging with explicit options
    /// </summary>
    public static IServiceCollection AddMarventaKafka(
        this IServiceCollection services,
        Action<KafkaOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IMessageBus, KafkaMessageBus>();

        return services;
    }

    /// <summary>
    /// Adds Kafka message handler as a hosted service
    /// </summary>
    public static IServiceCollection AddKafkaHandler<THandler, TMessage>(this IServiceCollection services)
        where THandler : BaseKafkaHandler<TMessage>
        where TMessage : class
    {
        services.AddHostedService<THandler>();
        return services;
    }

    /// <summary>
    /// Adds MassTransit with InMemory transport for testing purposes
    /// </summary>
    public static IServiceCollection AddMarventaInMemoryMessaging(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddScoped<IMessageBus, RabbitMqMessageBus>();

        var assembliesToScan = assemblies?.Any() == true
            ? assemblies
            : new[] { Assembly.GetExecutingAssembly(), Assembly.GetCallingAssembly() };

        services.AddMassTransit(x =>
        {
            x.AddConsumers(assembliesToScan);

            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    /// <summary>
    /// Advanced RabbitMQ configuration with custom options
    /// </summary>
    public static IServiceCollection AddMarventaRabbitMQ(
        this IServiceCollection services,
        Action<IBusRegistrationConfigurator> configure,
        Action<IRabbitMqBusFactoryConfigurator> configureRabbitMq)
    {
        services.AddScoped<IMessageBus, RabbitMqMessageBus>();

        services.AddMassTransit(x =>
        {
            configure(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("amqp://guest:guest@localhost:5672/");

                cfg.UseMessageRetry(r => r.Immediate(3));
                cfg.UseCircuitBreaker(cb => cb.TripThreshold = 5);

                configureRabbitMq(cfg);

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
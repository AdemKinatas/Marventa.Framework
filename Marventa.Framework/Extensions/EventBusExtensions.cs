using Marventa.Framework.Configuration;

using Marventa.Framework.Features.EventBus.Abstractions;
using Marventa.Framework.Features.EventBus.Kafka;
using Marventa.Framework.Features.EventBus.RabbitMQ;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Marventa.Framework.Extensions;

/// <summary>
/// Provides extension methods for configuring event bus and messaging services.
/// Supports RabbitMQ, Kafka, and MassTransit integration.
/// </summary>
public static class EventBusExtensions
{
    /// <summary>
    /// Adds RabbitMQ event bus services with custom implementation.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaRabbitMQ(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.HasSection(ConfigurationKeys.RabbitMQ))
        {
            return services;
        }

        var host = configuration.GetValueOrDefault(
            ConfigurationKeys.RabbitMQHost,
            ConfigurationKeys.Defaults.RabbitMQHost);

        var username = configuration.GetValueOrDefault(
            ConfigurationKeys.RabbitMQUsername,
            ConfigurationKeys.Defaults.RabbitMQUsername);

        var password = configuration.GetValueOrDefault(
            ConfigurationKeys.RabbitMQPassword,
            ConfigurationKeys.Defaults.RabbitMQPassword);

        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
        {
            HostName = host,
            UserName = username,
            Password = password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        });

        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddSingleton<IEventBus, RabbitMqEventBus>();

        return services;
    }

    /// <summary>
    /// Adds Kafka producer and consumer services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaKafka(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.HasSection(ConfigurationKeys.Kafka))
        {
            return services;
        }

        var bootstrapServers = configuration.GetValueOrDefault(
            ConfigurationKeys.KafkaBootstrapServers,
            ConfigurationKeys.Defaults.KafkaBootstrapServers);

        var groupId = configuration.GetValueOrDefault(
            ConfigurationKeys.KafkaGroupId,
            ConfigurationKeys.Defaults.KafkaGroupId);

        services.AddSingleton<IKafkaProducer>(_ => new KafkaProducer(bootstrapServers));

        services.AddSingleton<IKafkaConsumer>(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<KafkaConsumer>>();
            return new KafkaConsumer(bootstrapServers, groupId, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds MassTransit with RabbitMQ transport and auto-discovery of consumers.
    /// Scans provided assemblies for consumer implementations.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="assemblies">Assemblies to scan for consumer implementations.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaMassTransit(
        this IServiceCollection services,
        IConfiguration configuration,
        params System.Reflection.Assembly[] assemblies)
    {
        if (!configuration.IsSectionEnabled(ConfigurationKeys.MassTransit))
        {
            return services;
        }

        var host = configuration.GetValueOrDefault(
            ConfigurationKeys.RabbitMQHost,
            ConfigurationKeys.Defaults.RabbitMQHost);

        var virtualHost = configuration.GetValueOrDefault(
            ConfigurationKeys.RabbitMQVirtualHost,
            ConfigurationKeys.Defaults.RabbitMQVirtualHost);

        var username = configuration.GetValueOrDefault(
            ConfigurationKeys.RabbitMQUsername,
            ConfigurationKeys.Defaults.RabbitMQUsername);

        var password = configuration.GetValueOrDefault(
            ConfigurationKeys.RabbitMQPassword,
            ConfigurationKeys.Defaults.RabbitMQPassword);

        var assembliesToScan = assemblies.Length > 0
            ? assemblies
            : new[] { System.Reflection.Assembly.GetCallingAssembly() };

        services.AddMassTransit(busConfigurator =>
        {
            // Register all consumers from specified assemblies
            foreach (var assembly in assembliesToScan)
            {
                busConfigurator.AddConsumers(assembly);
            }

            // Configure RabbitMQ transport
            busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
            {
                rabbitConfigurator.Host(host, virtualHost, hostConfigurator =>
                {
                    hostConfigurator.Username(username);
                    hostConfigurator.Password(password);
                });

                // Auto-configure endpoints based on consumer definitions
                rabbitConfigurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    /// <summary>
    /// Adds all available event bus services based on configuration.
    /// Configures RabbitMQ, Kafka, and MassTransit if their sections are present.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="assemblies">Assemblies to scan for MassTransit consumers.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddMarventaEventBus(
        this IServiceCollection services,
        IConfiguration configuration,
        params System.Reflection.Assembly[] assemblies)
    {
        services.AddMarventaRabbitMQ(configuration);
        services.AddMarventaKafka(configuration);
        services.AddMarventaMassTransit(configuration, assemblies);

        return services;
    }
}

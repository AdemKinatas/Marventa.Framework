using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Marventa.Framework.Core.Interfaces.Data;
using Marventa.Framework.Core.Interfaces.MultiTenancy;
using Marventa.Framework.Core.Interfaces.Sagas;
using Marventa.Framework.Core.Interfaces.MachineLearning;
using Marventa.Framework.Core.Interfaces.Services;
using Marventa.Framework.Core.Interfaces.Messaging.Outbox;
using Marventa.Framework.Core.Interfaces.Projections;
using Marventa.Framework.Core.Models;
using Marventa.Framework.Infrastructure.Multitenancy;
using Marventa.Framework.Infrastructure.Authorization;
using Marventa.Framework.Infrastructure.Sagas;
using Marventa.Framework.Infrastructure.Services.FileServices;
using Marventa.Framework.Infrastructure.Search;
using Marventa.Framework.Infrastructure.Messaging;
using Marventa.Framework.Infrastructure.Projections;

namespace Marventa.Framework.Web.Extensions;

/// <summary>
/// Enterprise architecture service extensions (Saga, Multi-tenancy, CQRS, Event-driven)
/// </summary>
internal static class ServiceCollectionExtensionsEnterprise
{
    internal static IServiceCollection AddEnterpriseArchitectureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // Saga Pattern Implementation
        if (options.EnableSagas)
        {
            services.AddScoped<ISagaManager, SagaManager>();
            services.AddScoped(typeof(ISagaRepository<>), typeof(SimpleSagaRepository<>));

            // Register example saga orchestrator
            services.AddScoped<ISagaOrchestrator<OrderSaga>, OrderSagaOrchestrator>();
        }

        // Multi-tenancy services
        if (options.EnableMultiTenancy)
        {
            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ITenantResolver<Microsoft.AspNetCore.Http.HttpContext>, TenantResolver>();
            services.AddScoped<ITenantStore, TenantStore>();
            services.AddScoped<ITenantAuthorization, TenantAuthorizationService>();
        }

        // CQRS pattern with MediatR and Pipeline Behaviors
        if (options.EnableCQRS)
        {
            services.AddCqrsServices(options);
        }

        // Event-driven architecture
        if (options.EnableEventDriven)
        {
            // Event bus and handlers would go here
        }

        return services;
    }

    private static IServiceCollection AddCqrsServices(
        this IServiceCollection services,
        MarventaFrameworkOptions options)
    {
        var cqrsOptions = options.CqrsOptions;

        if (cqrsOptions.Assemblies == null || !cqrsOptions.Assemblies.Any())
        {
            throw new InvalidOperationException(
                "CQRS is enabled but no assemblies are configured. " +
                "Please add assemblies to CqrsOptions.Assemblies in your configuration.");
        }

        // Add MediatR with handlers from configured assemblies
        services.AddMediatR(cfg =>
        {
            foreach (var assembly in cqrsOptions.Assemblies)
            {
                cfg.RegisterServicesFromAssembly(assembly);
            }

            // Add Marventa pipeline behaviors based on configuration
            if (cqrsOptions.EnableValidationBehavior)
            {
                cfg.AddOpenBehavior(typeof(Marventa.Framework.Application.Behaviors.ValidationBehavior<,>));
            }

            if (cqrsOptions.EnableLoggingBehavior)
            {
                cfg.AddOpenBehavior(typeof(Marventa.Framework.Application.Behaviors.LoggingBehavior<,>));
            }

            if (cqrsOptions.EnableTransactionBehavior)
            {
                cfg.AddOpenBehavior(typeof(Marventa.Framework.Application.Behaviors.TransactionBehavior<,>));
            }
        });

        // Add FluentValidation validators if ValidationBehavior is enabled
        if (cqrsOptions.EnableValidationBehavior)
        {
            foreach (var assembly in cqrsOptions.Assemblies)
            {
                services.AddValidatorsFromAssembly(assembly);
            }
        }

        // Add Unit of Work for transaction management
        services.AddScoped<IUnitOfWork, Marventa.Framework.Infrastructure.Data.UnitOfWork>();

        return services;
    }

    internal static IServiceCollection AddSearchAndAiServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // ML Services
        if (options.EnableML)
        {
            services.AddScoped<IMarventaML, MockMLService>();
        }

        // Elasticsearch Services
        if (options.EnableSearch)
        {
            services.AddHttpClient<ElasticsearchService>();
            services.AddScoped<ISearchService, ElasticsearchService>();
        }

        return services;
    }

    internal static IServiceCollection AddBusinessServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // Business services like e-commerce, payments, shipping
        return services;
    }

    internal static IServiceCollection AddPerformanceServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // Performance services like distributed locking, circuit breaker, batch operations
        return services;
    }

    internal static IServiceCollection AddMonitoringServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // Monitoring services like analytics, observability, tracking
        return services;
    }

    internal static IServiceCollection AddBackgroundProcessingServices(
        this IServiceCollection services,
        IConfiguration configuration,
        MarventaFrameworkOptions options)
    {
        // Outbox/Inbox Pattern (Transactional messaging)
        if (options.EnableMessaging)
        {
            services.AddScoped<IOutboxService, OutboxService>();
            services.AddScoped<IInboxService, InboxService>();
            services.AddScoped<ITransactionalMessageService, TransactionalMessageService>();
            // Note: OutboxDispatcher is not a HostedService, users should configure their own background processor
        }

        // Projections (Event Sourcing read models)
        if (options.EnableProjections)
        {
            services.AddScoped(typeof(IProjectionRepository<>), typeof(MongoProjectionRepository<>));
            services.AddScoped<IProjectionManager, ProjectionManager>();
        }

        return services;
    }
}

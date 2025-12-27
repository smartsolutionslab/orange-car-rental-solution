using Eventuous.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.CommandServices;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.EventSourcing;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Projections;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering event sourcing services.
/// </summary>
public static class EventSourcingExtensions
{
    /// <summary>
    /// Registers event sourcing services for the Customers domain.
    /// Includes event publisher, event-sourced repository, and command service.
    /// </summary>
    public static IServiceCollection AddCustomerEventPublisher(this IServiceCollection services)
    {
        // Register the null publisher by default
        // Replace with actual implementation (Azure Service Bus, RabbitMQ, etc.) when configured
        services.AddScoped<ICustomerEventPublisher, NullCustomerEventPublisher>();

        // Register event-sourced repository
        services.AddScoped<IEventSourcedCustomerRepository, EventSourcedCustomerRepository>();

        // Register command service for event-sourced operations
        services.AddScoped<ICustomerCommandService, CustomerCommandService>();

        return services;
    }

    /// <summary>
    /// Registers the Customer read model projection.
    /// This projection subscribes to Customer events and updates the EF Core database.
    /// </summary>
    public static IServiceCollection AddCustomerReadModelProjection(this IServiceCollection services)
    {
        services.AddSingleton<IEventHandler, CustomerReadModelProjection>();
        return services;
    }
}

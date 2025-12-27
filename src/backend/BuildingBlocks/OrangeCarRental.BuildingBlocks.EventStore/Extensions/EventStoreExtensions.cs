using Eventuous;
using Eventuous.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.EventStore.TypeMapping;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.EventStore.Extensions;

/// <summary>
/// Extension methods for configuring Eventuous event store with SQL Server.
/// </summary>
public static class EventStoreExtensions
{
    /// <summary>
    /// Adds Eventuous SQL Server event store to the service collection.
    /// Reads configuration from the "SqlServerStore" section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section for SqlServerStore.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOrangeCarRentalEventStore(
        this IServiceCollection services,
        IConfigurationSection configuration)
    {
        // Register all domain events with type mapper
        EventTypeMapper.RegisterAllEvents();

        // Configure SQL Server event store from configuration
        // This registers SqlServerStore as IEventReader and IEventWriter
        services.AddEventuousSqlServer(configuration);

        return services;
    }

    /// <summary>
    /// Adds Eventuous SQL Server event store using a connection string.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The SQL Server connection string.</param>
    /// <param name="schema">The database schema for event store tables (default: "events").</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOrangeCarRentalEventStore(
        this IServiceCollection services,
        string connectionString,
        string schema = "events")
    {
        // Register all domain events with type mapper
        EventTypeMapper.RegisterAllEvents();

        // Configure SQL Server event store with connection string
        services.AddSingleton(new SqlServerStoreOptions
        {
            ConnectionString = connectionString,
            Schema = schema
        });
        services.AddSingleton<SqlServerStore>();

        // Register SqlServerStore as IEventReader and IEventWriter
        services.AddSingleton<IEventReader>(sp => sp.GetRequiredService<SqlServerStore>());
        services.AddSingleton<IEventWriter>(sp => sp.GetRequiredService<SqlServerStore>());

        return services;
    }

    /// <summary>
    /// Adds Customer event sourcing services.
    /// Call this to register Customer domain events.
    /// </summary>
    public static IServiceCollection AddCustomerEventSourcing(this IServiceCollection services)
    {
        EventTypeMapper.RegisterCustomerEvents();
        return services;
    }

    /// <summary>
    /// Adds Reservation event sourcing services.
    /// Call this to register Reservation domain events.
    /// </summary>
    public static IServiceCollection AddReservationEventSourcing(this IServiceCollection services)
    {
        EventTypeMapper.RegisterReservationEvents();
        return services;
    }
}

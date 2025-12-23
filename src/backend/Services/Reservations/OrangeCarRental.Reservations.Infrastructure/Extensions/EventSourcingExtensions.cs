using Eventuous.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.CommandServices;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.EventSourcing;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Projections;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering event sourcing services.
/// </summary>
public static class EventSourcingExtensions
{
    /// <summary>
    /// Registers event sourcing services for the Reservations domain.
    /// Includes event publisher, event-sourced repository, and command service.
    /// </summary>
    public static IServiceCollection AddReservationEventPublisher(this IServiceCollection services)
    {
        // Register the null publisher by default
        // Replace with actual implementation (Azure Service Bus, RabbitMQ, etc.) when configured
        services.AddScoped<IReservationEventPublisher, NullReservationEventPublisher>();

        // Register event-sourced repository
        services.AddScoped<IEventSourcedReservationRepository, EventSourcedReservationRepository>();

        // Register command service for event-sourced operations
        services.AddScoped<IReservationCommandService, ReservationCommandService>();

        return services;
    }

    /// <summary>
    /// Registers the Reservation read model projection.
    /// This projection subscribes to Reservation events and updates the EF Core database.
    /// </summary>
    public static IServiceCollection AddReservationReadModelProjection(this IServiceCollection services)
    {
        services.AddSingleton<IEventHandler, ReservationReadModelProjection>();
        return services;
    }
}

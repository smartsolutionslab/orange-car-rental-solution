using Eventuous;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.EventStore.TypeMapping;

/// <summary>
/// Registers all domain events with Eventuous type mapper for serialization.
/// </summary>
public static class EventTypeMapper
{
    /// <summary>
    /// Registers all domain events from Customers and Reservations domains.
    /// Call this at application startup.
    /// </summary>
    public static void RegisterAllEvents()
    {
        RegisterCustomerEvents();
        RegisterReservationEvents();
    }

    /// <summary>
    /// Registers Customer domain events.
    /// </summary>
    public static void RegisterCustomerEvents()
    {
        TypeMap.RegisterKnownEventTypes(typeof(CustomerRegistered).Assembly);
    }

    /// <summary>
    /// Registers Reservation domain events.
    /// </summary>
    public static void RegisterReservationEvents()
    {
        TypeMap.RegisterKnownEventTypes(typeof(ReservationCreated).Assembly);
    }
}

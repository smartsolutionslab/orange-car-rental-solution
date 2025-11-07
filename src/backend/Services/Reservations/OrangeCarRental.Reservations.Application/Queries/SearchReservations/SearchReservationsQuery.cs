using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;

/// <summary>
///     Query to search reservations with filters and pagination.
/// </summary>
public sealed record SearchReservationsQuery(
    string? Status = null,
    CustomerIdentifier? CustomerId = null,
    VehicleIdentifier? VehicleId = null,
    DateTime? PickupDateFrom = null,
    DateTime? PickupDateTo = null,
    int PageNumber = 1,
    int PageSize = 50
);

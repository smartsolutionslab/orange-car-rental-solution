namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;

/// <summary>
///     Query to search reservations with filters and pagination.
/// </summary>
public sealed record SearchReservationsQuery(
    string? Status = null,
    Guid? CustomerId = null,
    Guid? VehicleId = null,
    DateTime? PickupDateFrom = null,
    DateTime? PickupDateTo = null,
    int PageNumber = 1,
    int PageSize = 50
);

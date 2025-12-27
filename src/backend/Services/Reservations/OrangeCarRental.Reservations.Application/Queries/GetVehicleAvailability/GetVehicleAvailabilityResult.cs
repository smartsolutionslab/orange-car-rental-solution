namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetVehicleAvailability;

/// <summary>
///     Result containing list of vehicle IDs that are booked during the requested period.
/// </summary>
public sealed record GetVehicleAvailabilityResult(
    IReadOnlyList<Guid> BookedVehicleIds,
    DateOnly PickupDate,
    DateOnly ReturnDate
);

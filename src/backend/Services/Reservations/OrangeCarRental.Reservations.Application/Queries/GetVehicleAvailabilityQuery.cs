using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries;

/// <summary>
///     Query to get list of vehicle IDs that are booked (unavailable) during a specific period.
///     Used by Fleet service to determine vehicle availability.
/// </summary>
public sealed record GetVehicleAvailabilityQuery(
    DateOnly PickupDate,
    DateOnly ReturnDate
) : IQuery<GetVehicleAvailabilityResult>;

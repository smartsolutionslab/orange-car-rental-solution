using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Parameters for searching reservations with filtering, sorting, and pagination.
/// </summary>
public sealed record ReservationSearchParameters(
    ReservationStatus? Status,
    CustomerIdentifier? CustomerId,
    SearchTerm? CustomerName,
    VehicleIdentifier? VehicleId,
    VehicleCategory? Category,
    LocationCode? PickupLocationCode,
    DateRange? PickupDateRange,
    PriceRange? PriceRange,
    PagingInfo Paging,
    SortingInfo Sorting) : SearchParameters(Paging, Sorting);

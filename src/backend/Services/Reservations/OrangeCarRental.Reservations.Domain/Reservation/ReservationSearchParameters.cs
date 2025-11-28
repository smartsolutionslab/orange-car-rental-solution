using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Parameters for searching reservations with filtering, sorting, and pagination.
/// </summary>
public sealed record ReservationSearchParameters(
    ReservationStatus? Status,
    CustomerIdentifier? CustomerId,
    string? CustomerName,
    VehicleIdentifier? VehicleId,
    string? CategoryCode,
    string? PickupLocationCode,
    DateOnly? PickupDateFrom,
    DateOnly? PickupDateTo,
    decimal? PriceMin,
    decimal? PriceMax,
    string? SortBy,
    bool SortDescending,
    int PageNumber,
    int PageSize)
    : SearchParameters(PageNumber, PageSize, SortBy, SortDescending)
{
    /// <summary>
    ///     Validates the search parameters.
    /// </summary>
    public override void Validate()
    {
        base.Validate();

        if (PriceMin.HasValue && PriceMin < 0)
            throw new ArgumentOutOfRangeException(nameof(PriceMin), "Minimum price cannot be negative");

        if (PriceMax.HasValue && PriceMax < 0)
            throw new ArgumentOutOfRangeException(nameof(PriceMax), "Maximum price cannot be negative");

        if (PriceMin.HasValue && PriceMax.HasValue && PriceMin > PriceMax)
            throw new ArgumentException("Minimum price cannot be greater than maximum price");

        if (PickupDateFrom.HasValue && PickupDateTo.HasValue && PickupDateFrom > PickupDateTo)
            throw new ArgumentException("PickupDateFrom cannot be after PickupDateTo");
    }
}

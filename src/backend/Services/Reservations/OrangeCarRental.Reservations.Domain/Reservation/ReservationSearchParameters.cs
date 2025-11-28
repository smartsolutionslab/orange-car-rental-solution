using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Parameters for searching reservations with filtering, sorting, and pagination.
/// </summary>
public sealed record ReservationSearchParameters : SearchParameters
{
    public ReservationStatus? Status { get; init; }
    public CustomerIdentifier? CustomerId { get; init; }
    public SearchTerm? CustomerName { get; init; }
    public VehicleIdentifier? VehicleId { get; init; }
    public CategoryCode? CategoryCode { get; init; }
    public LocationCode? PickupLocationCode { get; init; }
    public DateRange? PickupDateRange { get; init; }
    public PriceRange? PriceRange { get; init; }

    public ReservationSearchParameters(PagingInfo paging) : base(paging, SortingInfo.None)
    {
    }

    public ReservationSearchParameters(PagingInfo paging, SortingInfo sorting) : base(paging, sorting)
    {
    }

    /// <summary>
    ///     Creates search parameters with default paging.
    /// </summary>
    public static ReservationSearchParameters Default() => new(PagingInfo.Default);

    /// <summary>
    ///     Creates search parameters with specific paging.
    /// </summary>
    public static ReservationSearchParameters WithPaging(int pageNumber, int pageSize) =>
        new(PagingInfo.Create(pageNumber, pageSize));

    /// <summary>
    ///     Creates search parameters filtered by status.
    /// </summary>
    public static ReservationSearchParameters ForStatus(ReservationStatus status, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { Status = status };

    /// <summary>
    ///     Creates search parameters filtered by customer.
    /// </summary>
    public static ReservationSearchParameters ForCustomer(CustomerIdentifier customerId, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { CustomerId = customerId };

    /// <summary>
    ///     Creates search parameters filtered by vehicle.
    /// </summary>
    public static ReservationSearchParameters ForVehicle(VehicleIdentifier vehicleId, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { VehicleId = vehicleId };

    /// <summary>
    ///     Creates search parameters filtered by pickup location.
    /// </summary>
    public static ReservationSearchParameters ForLocation(LocationCode locationCode, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { PickupLocationCode = locationCode };

    /// <summary>
    ///     Creates search parameters filtered by pickup date range.
    /// </summary>
    public static ReservationSearchParameters ForDateRange(DateRange dateRange, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { PickupDateRange = dateRange };

    /// <summary>
    ///     Creates search parameters filtered by price range.
    /// </summary>
    public static ReservationSearchParameters ForPriceRange(PriceRange priceRange, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { PriceRange = priceRange };

    /// <summary>
    ///     Validates the search parameters.
    /// </summary>
    public override void Validate()
    {
        base.Validate();
        // DateRange and PriceRange validate themselves on creation
    }
}

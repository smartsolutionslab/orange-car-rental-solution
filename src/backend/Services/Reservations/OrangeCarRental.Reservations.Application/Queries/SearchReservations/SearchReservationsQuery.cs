using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;

/// <summary>
///     Query to search reservations with filters, sorting, and pagination.
/// </summary>
public sealed record SearchReservationsQuery(
    // Status filter
    ReservationStatus? Status = null,

    // Customer filters
    CustomerIdentifier? CustomerId = null,
    SearchTerm? CustomerName = null,

    // Vehicle filters
    VehicleIdentifier? VehicleId = null,
    VehicleCategory? Category = null,

    // Location filter
    LocationCode? PickupLocationCode = null,

    // Date range filters
    DateRange? DateRange = null,

    // Price range filters (gross amount in EUR)
    PriceRange? PriceRange = null,

    // Sorting
    string? SortBy = null, // Options: "PickupDate", "Price", "Status", "CreatedDate"
    bool SortDescending = false,

    // Pagination - uses standardized defaults from PagedResult
    int PageNumber = 1,
    int PageSize = PagedResult<object>.DefaultPageSize
) : IQuery<SearchReservationsResult>;

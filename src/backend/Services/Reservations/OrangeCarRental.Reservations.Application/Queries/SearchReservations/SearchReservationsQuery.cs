using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;

/// <summary>
///     Query to search reservations with filters, sorting, and pagination.
/// </summary>
public sealed record SearchReservationsQuery(
    // Status filter
    string? Status = null,

    // Customer filters
    CustomerIdentifier? CustomerId = null,
    string? CustomerName = null,

    // Vehicle filters
    VehicleIdentifier? VehicleId = null,
    string? CategoryCode = null,

    // Location filter
    string? PickupLocationCode = null,

    // Date range filters
    DateTime? PickupDateFrom = null,
    DateTime? PickupDateTo = null,

    // Price range filters (gross amount in EUR)
    decimal? PriceMin = null,
    decimal? PriceMax = null,

    // Sorting
    string? SortBy = null, // Options: "PickupDate", "Price", "Status", "CreatedDate"
    bool SortDescending = false,

    // Pagination
    int PageNumber = 1,
    int PageSize = 50
) : IQuery<SearchReservationsResult>;

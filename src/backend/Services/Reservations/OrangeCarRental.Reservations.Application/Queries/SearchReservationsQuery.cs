using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries;

/// <summary>
///     Query to search reservations with filters, sorting, and pagination.
///     Uses value objects for type-safe filtering.
/// </summary>
public sealed record SearchReservationsQuery(
    // Status filter
    ReservationStatus? Status,

    // Customer filters
    CustomerIdentifier? CustomerId,
    SearchTerm? CustomerName,

    // Vehicle filters
    VehicleIdentifier? VehicleId,
    VehicleCategory? Category,

    // Location filter
    LocationCode? PickupLocationCode,

    // Date range filters
    DateRange? DateRange,

    // Price range filters (gross amount in EUR)
    PriceRange? PriceRange,

    // Pagination and sorting
    PagingInfo Paging,
    SortingInfo Sorting
) : IQuery<PagedResult<ReservationDto>>;

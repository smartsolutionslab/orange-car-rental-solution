using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;

/// <summary>
///     Paginated result for reservation search.
/// </summary>
public sealed record SearchReservationsResult(
    IReadOnlyList<ReservationDto> Reservations,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages)
{
    /// <summary>
    ///     Indicates if there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    ///     Indicates if there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

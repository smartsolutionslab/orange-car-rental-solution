using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.SearchReservations;

/// <summary>
/// Paginated result for reservation search.
/// </summary>
public sealed record SearchReservationsResult(
    List<ReservationDto> Reservations,
    int TotalCount,
    int PageNumber,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

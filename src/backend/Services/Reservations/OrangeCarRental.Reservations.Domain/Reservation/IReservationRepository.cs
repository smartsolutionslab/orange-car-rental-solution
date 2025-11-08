namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Repository interface for Reservation aggregate.
/// </summary>
public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(ReservationIdentifier id, CancellationToken cancellationToken = default);

    Task<List<Reservation>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<(List<Reservation> Reservations, int TotalCount)> SearchAsync(
        ReservationStatus? status = null,
        Guid? customerId = null,
        string? customerName = null,
        Guid? vehicleId = null,
        string? categoryCode = null,
        string? pickupLocationCode = null,
        DateOnly? pickupDateFrom = null,
        DateOnly? pickupDateTo = null,
        decimal? priceMin = null,
        decimal? priceMax = null,
        string? sortBy = null,
        string? sortDirection = "asc",
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);

    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);

    Task DeleteAsync(ReservationIdentifier id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

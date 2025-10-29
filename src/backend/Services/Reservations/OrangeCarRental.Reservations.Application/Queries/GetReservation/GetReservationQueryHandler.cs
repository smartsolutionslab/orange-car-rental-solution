using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Aggregates;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetReservation;

/// <summary>
/// Handler for GetReservationQuery.
/// Retrieves a reservation by ID and maps to DTO.
/// </summary>
public sealed class GetReservationQueryHandler
{
    // TODO: Inject IReservationRepository when database is implemented
    // For now, we'll return sample data for demonstration

    public Task<ReservationDto?> HandleAsync(
        GetReservationQuery query,
        CancellationToken cancellationToken = default)
    {
        // TODO: Fetch from repository
        // var reservation = await _repository.GetByIdAsync(query.ReservationId, cancellationToken);
        // if (reservation == null) return null;

        // For now, return null (not found)
        // In a real implementation, this would fetch from the database
        return Task.FromResult<ReservationDto?>(null);
    }

    private static ReservationDto MapToDto(Reservation reservation)
    {
        return new ReservationDto
        {
            ReservationId = reservation.Id.Value,
            VehicleId = reservation.VehicleId,
            CustomerId = reservation.CustomerId,
            PickupDate = reservation.Period.PickupDate,
            ReturnDate = reservation.Period.ReturnDate,
            RentalDays = reservation.Period.Days,
            TotalPriceNet = reservation.TotalPrice.NetAmount,
            TotalPriceVat = reservation.TotalPrice.VatAmount,
            TotalPriceGross = reservation.TotalPrice.GrossAmount,
            Currency = reservation.TotalPrice.Currency.Code,
            Status = reservation.Status.ToString(),
            CancellationReason = reservation.CancellationReason,
            CreatedAt = reservation.CreatedAt,
            ConfirmedAt = reservation.ConfirmedAt,
            CancelledAt = reservation.CancelledAt,
            CompletedAt = reservation.CompletedAt
        };
    }
}

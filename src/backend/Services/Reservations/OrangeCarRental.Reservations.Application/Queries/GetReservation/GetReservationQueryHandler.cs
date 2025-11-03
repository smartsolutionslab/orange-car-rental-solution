using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetReservation;

/// <summary>
/// Handler for GetReservationQuery.
/// Retrieves a reservation by ID and maps to DTO.
/// </summary>
public sealed class GetReservationQueryHandler(IReservationRepository reservations)
{
    public async Task<ReservationDto?> HandleAsync(
        GetReservationQuery query,
        CancellationToken cancellationToken = default)
    {
        var reservationId = ReservationIdentifier.From(query.ReservationId);
        var reservation = await reservations.GetByIdAsync(reservationId, cancellationToken);

        return reservation == null ? null : MapToDto(reservation);
    }

    private static ReservationDto MapToDto(Reservation reservation)
    {
        return new ReservationDto
        {
            ReservationId = reservation.Id.Value,
            VehicleId = reservation.VehicleId,
            CustomerId = reservation.CustomerId,
            PickupDate = reservation.Period.PickupDate.ToDateTime(TimeOnly.MinValue),
            ReturnDate = reservation.Period.ReturnDate.ToDateTime(TimeOnly.MinValue),
            PickupLocationCode = reservation.PickupLocationCode.Value,
            DropoffLocationCode = reservation.DropoffLocationCode.Value,
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

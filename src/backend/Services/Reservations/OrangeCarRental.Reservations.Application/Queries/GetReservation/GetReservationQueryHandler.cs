using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetReservation;

/// <summary>
///     Handler for GetReservationQuery.
///     Retrieves a reservation by ID and maps to DTO.
/// </summary>
public sealed class GetReservationQueryHandler(IReservationRepository reservations)
    : IQueryHandler<GetReservationQuery, ReservationDto>
{
    public async Task<ReservationDto> HandleAsync(
        GetReservationQuery query,
        CancellationToken cancellationToken = default)
    {
        var reservation = await reservations.GetByIdAsync(query.ReservationId, cancellationToken);

        return MapToDto(reservation);
    }

    private static ReservationDto MapToDto(Reservation reservation)
    {
        return new ReservationDto(
            reservation.Id.Value,
            reservation.VehicleId.Value,
            reservation.CustomerId.Value,
            reservation.Period.PickupDate,
            reservation.Period.ReturnDate,
            reservation.PickupLocationCode.Value,
            reservation.DropoffLocationCode.Value,
            reservation.Period.Days,
            reservation.TotalPrice.NetAmount,
            reservation.TotalPrice.VatAmount,
            reservation.TotalPrice.GrossAmount,
            reservation.TotalPrice.Currency.Code,
            reservation.Status.ToString(),
            reservation.CancellationReason,
            reservation.CreatedAt,
            reservation.ConfirmedAt,
            reservation.CancelledAt,
            reservation.CompletedAt
        );
    }
}

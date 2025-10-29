using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Aggregates;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetReservation;

/// <summary>
/// Handler for GetReservationQuery.
/// Retrieves a reservation by ID and maps to DTO.
/// </summary>
public sealed class GetReservationQueryHandler
{
    private readonly IReservationRepository _repository;

    public GetReservationQueryHandler(IReservationRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ReservationDto?> HandleAsync(
        GetReservationQuery query,
        CancellationToken cancellationToken = default)
    {
        var reservationId = ReservationIdentifier.From(query.ReservationId);
        var reservation = await _repository.GetByIdAsync(reservationId, cancellationToken);

        return reservation == null ? null : MapToDto(reservation);
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

using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.EventSourcing;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.CommandServices;

/// <summary>
/// Implementation of IReservationCommandService using event sourcing.
/// Orchestrates loading aggregates from the event store, executing commands, and saving events.
/// </summary>
public sealed class ReservationCommandService : IReservationCommandService
{
    private readonly IEventSourcedReservationRepository _repository;

    public ReservationCommandService(IEventSourcedReservationRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<Reservation> CreateAsync(
        VehicleIdentifier vehicleId,
        CustomerIdentifier customerId,
        BookingPeriod period,
        LocationCode pickupLocationCode,
        LocationCode dropoffLocationCode,
        Money totalPrice,
        CancellationToken cancellationToken = default)
    {
        var reservation = new Reservation();
        reservation.Create(vehicleId, customerId, period, pickupLocationCode, dropoffLocationCode, totalPrice);

        await _repository.SaveAsync(reservation, cancellationToken);

        return reservation;
    }

    /// <inheritdoc />
    public async Task<Reservation> ConfirmAsync(
        ReservationIdentifier reservationId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _repository.LoadAsync(reservationId, cancellationToken);

        if (!reservation.State.HasBeenCreated)
            throw new InvalidOperationException($"Reservation with ID '{reservationId.Value}' not found.");

        reservation.Confirm();

        await _repository.SaveAsync(reservation, cancellationToken);

        return reservation;
    }

    /// <inheritdoc />
    public async Task<Reservation> CancelAsync(
        ReservationIdentifier reservationId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _repository.LoadAsync(reservationId, cancellationToken);

        if (!reservation.State.HasBeenCreated)
            throw new InvalidOperationException($"Reservation with ID '{reservationId.Value}' not found.");

        reservation.Cancel(reason);

        await _repository.SaveAsync(reservation, cancellationToken);

        return reservation;
    }

    /// <inheritdoc />
    public async Task<Reservation> ActivateAsync(
        ReservationIdentifier reservationId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _repository.LoadAsync(reservationId, cancellationToken);

        if (!reservation.State.HasBeenCreated)
            throw new InvalidOperationException($"Reservation with ID '{reservationId.Value}' not found.");

        reservation.MarkAsActive();

        await _repository.SaveAsync(reservation, cancellationToken);

        return reservation;
    }

    /// <inheritdoc />
    public async Task<Reservation> CompleteAsync(
        ReservationIdentifier reservationId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _repository.LoadAsync(reservationId, cancellationToken);

        if (!reservation.State.HasBeenCreated)
            throw new InvalidOperationException($"Reservation with ID '{reservationId.Value}' not found.");

        reservation.Complete();

        await _repository.SaveAsync(reservation, cancellationToken);

        return reservation;
    }

    /// <inheritdoc />
    public async Task<Reservation> MarkAsNoShowAsync(
        ReservationIdentifier reservationId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _repository.LoadAsync(reservationId, cancellationToken);

        if (!reservation.State.HasBeenCreated)
            throw new InvalidOperationException($"Reservation with ID '{reservationId.Value}' not found.");

        reservation.MarkAsNoShow();

        await _repository.SaveAsync(reservation, cancellationToken);

        return reservation;
    }
}

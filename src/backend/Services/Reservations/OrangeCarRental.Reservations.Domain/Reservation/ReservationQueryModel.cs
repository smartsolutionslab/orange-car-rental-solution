using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.EventSourcing;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation.Events;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Query model (state) for Reservation aggregate.
///     Built by applying domain events and used for both reads and writes.
/// </summary>
public sealed record ReservationQueryModel : QueryModel<ReservationQueryModel, ReservationIdentifier>
{
    public override ReservationIdentifier Id { get; init; } = ReservationIdentifier.Empty;
    public VehicleIdentifier? VehicleIdentifier { get; init; }
    public CustomerIdentifier? CustomerIdentifier { get; init; }
    public BookingPeriod? Period { get; init; }
    public LocationCode? PickupLocationCode { get; init; }
    public LocationCode? DropoffLocationCode { get; init; }
    public Money? TotalPrice { get; init; }
    public ReservationStatus Status { get; init; } = ReservationStatus.Pending;
    public string? CancellationReason { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? ConfirmedAtUtc { get; init; }
    public DateTime? CancelledAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public DateTime? ActivatedAtUtc { get; init; }

    /// <summary>
    ///     Indicates whether the aggregate has been created (has received ReservationCreated event).
    /// </summary>
    public override bool HasBeenCreated => Id != ReservationIdentifier.Empty;

    /// <summary>
    ///     Registers event handlers for building state from events.
    /// </summary>
    public ReservationQueryModel()
    {
        On<ReservationCreated>(Handle);
        On<ReservationConfirmed>(Handle);
        On<ReservationCancelled>(Handle);
        On<ReservationCompleted>(Handle);
        On<ReservationActivated>(Handle);
        On<ReservationMarkedNoShow>(Handle);
    }

    private static ReservationQueryModel Handle(ReservationQueryModel state, ReservationCreated @event) =>
        state with
        {
            Id = @event.ReservationId,
            VehicleIdentifier = @event.VehicleIdentifier,
            CustomerIdentifier = @event.CustomerIdentifier,
            Period = @event.Period,
            PickupLocationCode = @event.PickupLocationCode,
            DropoffLocationCode = @event.DropoffLocationCode,
            TotalPrice = @event.TotalPrice,
            Status = ReservationStatus.Pending,
            CreatedAtUtc = @event.CreatedAtUtc
        };

    private static ReservationQueryModel Handle(ReservationQueryModel state, ReservationConfirmed @event) =>
        state with
        {
            Status = ReservationStatus.Confirmed,
            ConfirmedAtUtc = @event.ConfirmedAtUtc
        };

    private static ReservationQueryModel Handle(ReservationQueryModel state, ReservationCancelled @event) =>
        state with
        {
            Status = ReservationStatus.Cancelled,
            CancellationReason = @event.CancellationReason,
            CancelledAtUtc = @event.CancelledAtUtc
        };

    private static ReservationQueryModel Handle(ReservationQueryModel state, ReservationCompleted @event) =>
        state with
        {
            Status = ReservationStatus.Completed,
            CompletedAtUtc = @event.CompletedAtUtc
        };

    private static ReservationQueryModel Handle(ReservationQueryModel state, ReservationActivated @event) =>
        state with
        {
            Status = ReservationStatus.Active,
            ActivatedAtUtc = @event.ActivatedAtUtc
        };

    private static ReservationQueryModel Handle(ReservationQueryModel state, ReservationMarkedNoShow @event) =>
        state with
        {
            Status = ReservationStatus.NoShow,
            CancellationReason = @event.Reason,
            CancelledAtUtc = @event.MarkedAtUtc
        };
}

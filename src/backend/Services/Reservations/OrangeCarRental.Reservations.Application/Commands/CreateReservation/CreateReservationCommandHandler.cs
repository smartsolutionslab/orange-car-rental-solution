using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Aggregates;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;

/// <summary>
/// Handler for CreateReservationCommand.
/// Creates a new pending reservation and returns the reservation details.
/// </summary>
public sealed class CreateReservationCommandHandler
{
    // TODO: Inject IReservationRepository when database is implemented
    // For now, we'll store in-memory for demonstration

    public Task<CreateReservationResult> HandleAsync(
        CreateReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validate and create booking period
        var period = BookingPeriod.Of(command.PickupDate, command.ReturnDate);

        // Create Money value object with German VAT (19%)
        var totalPrice = Money.Euro(command.TotalPriceNet);

        // Create the reservation aggregate
        var reservation = Reservation.Create(
            command.VehicleId,
            command.CustomerId,
            period,
            totalPrice
        );

        // TODO: Save to repository
        // await _repository.AddAsync(reservation, cancellationToken);

        // Map to result
        var result = new CreateReservationResult(
            reservation.Id.Value,
            reservation.Status.ToString(),
            reservation.TotalPrice.NetAmount,
            reservation.TotalPrice.VatAmount,
            reservation.TotalPrice.GrossAmount
        );

        return Task.FromResult(result);
    }
}

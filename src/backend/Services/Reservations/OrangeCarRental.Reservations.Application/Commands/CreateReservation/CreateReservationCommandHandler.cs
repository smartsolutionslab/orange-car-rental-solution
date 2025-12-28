using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;

/// <summary>
///     Handler for CreateReservationCommand.
///     Creates a new pending reservation via event sourcing and returns the reservation details.
/// </summary>
public sealed class CreateReservationCommandHandler(
    IEventSourcedReservationRepository repository,
    IPricingService pricingService)
    : ICommandHandler<CreateReservationCommand, CreateReservationResult>
{
    public async Task<CreateReservationResult> HandleAsync(
        CreateReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        var (vehicleId, customerId, vehicleCategory, bookingPeriod, pickupLocationCode, dropoffLocationCode, _) = command;

        // Determine the total price: either use provided value or calculate via Pricing API
        Money totalPrice;
        if (command.TotalPrice.HasValue)
        {
            totalPrice = command.TotalPrice.Value;
        }
        else
        {
            var priceCalculation = await pricingService.CalculatePriceAsync(
                vehicleCategory,
                bookingPeriod,
                pickupLocationCode,
                cancellationToken);

            totalPrice = Money.Euro(priceCalculation.TotalPriceNet);
        }

        // Create reservation aggregate and execute domain logic
        var reservation = new Reservation();
        reservation.Create(
            vehicleId,
            customerId,
            bookingPeriod,
            pickupLocationCode,
            dropoffLocationCode,
            totalPrice);

        // Persist events to event store
        await repository.SaveAsync(reservation, cancellationToken);

        return new CreateReservationResult(
            reservation.Id.Value,
            reservation.Status.ToString(),
            reservation.TotalPrice!.Value.NetAmount,
            reservation.TotalPrice!.Value.VatAmount,
            reservation.TotalPrice!.Value.GrossAmount
        );
    }
}

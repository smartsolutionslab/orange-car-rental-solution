using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;

/// <summary>
///     Handler for CreateReservationCommand.
///     Creates a new pending reservation and returns the reservation details.
/// </summary>
public sealed class CreateReservationCommandHandler(
    IReservationRepository reservations,
    IPricingService pricingService)
    : ICommandHandler<CreateReservationCommand, CreateReservationResult>
{
    public async Task<CreateReservationResult> HandleAsync(CreateReservationCommand command, CancellationToken cancellationToken = default)
    {
        var (vehicleId, customerId, vehicleCategory, bookingPeriod, pickupLocationCode, dropoffLocationCode, _) = command;

        // Determine the total price: either use provided value or calculate via Pricing API
        Money totalPrice;
        if (command.TotalPrice.HasValue)
        {
            // Use provided price (backward compatibility)
            totalPrice = command.TotalPrice.Value;
        }
        else
        {
            // Calculate price via Pricing API
            // Note: Pricing service still uses primitives, so we extract from value objects
            var priceCalculation = await pricingService.CalculatePriceAsync(
                vehicleCategory,
                bookingPeriod,
                pickupLocationCode,
                cancellationToken);

            totalPrice = Money.Euro(priceCalculation.TotalPriceNet);
        }

        // Create the reservation aggregate
        var reservation = Reservation.Create(
            vehicleId,
            customerId,
            bookingPeriod,
            pickupLocationCode,
            dropoffLocationCode,
            totalPrice
        );

        // Save to repository
        await reservations.AddAsync(reservation, cancellationToken);
        await reservations.SaveChangesAsync(cancellationToken);

        return new CreateReservationResult(
            reservation.Id.Value,
            reservation.Status.ToString(),
            reservation.TotalPrice.NetAmount,
            reservation.TotalPrice.VatAmount,
            reservation.TotalPrice.GrossAmount
        );
    }
}

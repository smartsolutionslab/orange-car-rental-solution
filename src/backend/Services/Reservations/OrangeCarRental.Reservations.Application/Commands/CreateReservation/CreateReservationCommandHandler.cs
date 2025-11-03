using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateReservation;

/// <summary>
/// Handler for CreateReservationCommand.
/// Creates a new pending reservation and returns the reservation details.
/// </summary>
public sealed class CreateReservationCommandHandler(
    IReservationRepository reservations,
    IPricingService pricingService)
{
    public async Task<CreateReservationResult> HandleAsync(
        CreateReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validate and create booking period
        var period = BookingPeriod.Of(command.PickupDate, command.ReturnDate);

        // Parse location codes
        var pickupLocationCode = LocationCode.Of(command.PickupLocationCode);
        var dropoffLocationCode = LocationCode.Of(command.DropoffLocationCode);

        // Determine the total price: either use provided value or calculate via Pricing API
        Money totalPrice;
        if (command.TotalPriceNet.HasValue)
        {
            // Use provided price (backward compatibility)
            totalPrice = Money.Euro(command.TotalPriceNet.Value);
        }
        else
        {
            // Calculate price via Pricing API
            var priceCalculation = await pricingService.CalculatePriceAsync(
                command.CategoryCode,
                command.PickupDate,
                command.ReturnDate,
                command.PickupLocationCode,
                cancellationToken);

            totalPrice = Money.Euro(priceCalculation.TotalPriceNet);
        }

        // Create the reservation aggregate
        var reservation = Reservation.Create(
            command.VehicleId,
            command.CustomerId,
            period,
            pickupLocationCode,
            dropoffLocationCode,
            totalPrice
        );

        // Save to repository
        await reservations.AddAsync(reservation, cancellationToken);
        await reservations.SaveChangesAsync(cancellationToken);

        // Map to result
        return new CreateReservationResult(
            reservation.Id.Value,
            reservation.Status.ToString(),
            reservation.TotalPrice.NetAmount,
            reservation.TotalPrice.VatAmount,
            reservation.TotalPrice.GrossAmount
        );
    }
}

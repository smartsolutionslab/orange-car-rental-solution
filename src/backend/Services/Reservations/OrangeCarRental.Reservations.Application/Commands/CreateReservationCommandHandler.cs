using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;

/// <summary>
///     Handler for CreateReservationCommand.
///     Creates a new pending reservation and persists it to the database.
/// </summary>
public sealed class CreateReservationCommandHandler(
    IReservationRepository repository,
    IPricingService pricingService,
    IUnitOfWork unitOfWork)
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

        // Create reservation using static factory method
        var reservation = Reservation.Create(
            vehicleId,
            customerId,
            bookingPeriod,
            pickupLocationCode,
            dropoffLocationCode,
            totalPrice);

        // Persist to database
        await repository.AddAsync(reservation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateReservationResult(
            reservation.Id.Value,
            reservation.Status.ToString(),
            reservation.TotalPrice.NetAmount,
            reservation.TotalPrice.VatAmount,
            reservation.TotalPrice.GrossAmount
        );
    }
}

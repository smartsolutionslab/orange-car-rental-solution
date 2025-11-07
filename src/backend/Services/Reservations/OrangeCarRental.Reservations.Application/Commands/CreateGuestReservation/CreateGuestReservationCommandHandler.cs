using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateGuestReservation;

/// <summary>
///     Handler for CreateGuestReservationCommand.
///     Handles guest booking by:
///     1. Registering the customer via the Customers API
///     2. Calculating the price via the Pricing API
///     3. Creating the reservation with the new customer ID
/// </summary>
public sealed class CreateGuestReservationCommandHandler(
    ICustomersService customersService,
    IReservationRepository reservations,
    IPricingService pricingService)
    : ICommandHandler<CreateGuestReservationCommand, CreateGuestReservationResult>
{
    public async Task<CreateGuestReservationResult> HandleAsync(
        CreateGuestReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Step 1: Register the customer via Customers API
        // Extract primitive values from value objects for the DTO
        var registerCustomerDto = new RegisterCustomerDto(
            command.FirstName.Value,
            command.LastName.Value,
            command.Email.Value,
            command.PhoneNumber.Value,
            command.DateOfBirth,
            command.Address.Street,
            command.Address.City.Value,
            command.Address.PostalCode.Value,
            command.Address.Country,
            command.DriversLicense.LicenseNumber,
            command.DriversLicense.IssueCountry,
            command.DriversLicense.IssueDate,
            command.DriversLicense.ExpiryDate
        );

        var customerId = await customersService.RegisterCustomerAsync(
            registerCustomerDto,
            cancellationToken);

        // Step 2: Calculate price via Pricing API
        // Note: Pricing service still uses primitives, so we extract from value objects
        var priceCalculation = await pricingService.CalculatePriceAsync(
            command.CategoryCode.Code,
            command.Period.PickupDate.ToDateTime(TimeOnly.MinValue),
            command.Period.ReturnDate.ToDateTime(TimeOnly.MinValue),
            command.PickupLocationCode.Value,
            cancellationToken);

        var totalPrice = Money.Euro(priceCalculation.TotalPriceNet);

        // Step 3: Create the reservation aggregate with the new customer ID
        var reservation = Reservation.Create(
            command.VehicleId.Value,
            customerId,
            command.Period,
            command.PickupLocationCode,
            command.DropoffLocationCode,
            totalPrice
        );

        // Step 5: Save to repository
        await reservations.AddAsync(reservation, cancellationToken);
        await reservations.SaveChangesAsync(cancellationToken);

        // Step 6: Return result with both customer and reservation IDs
        return new CreateGuestReservationResult(
            customerId,
            reservation.Id.Value,
            reservation.TotalPrice.NetAmount,
            reservation.TotalPrice.VatAmount,
            reservation.TotalPrice.GrossAmount,
            reservation.TotalPrice.Currency.Code
        );
    }
}

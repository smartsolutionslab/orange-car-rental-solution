using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands.CreateGuestReservation;

/// <summary>
/// Handler for CreateGuestReservationCommand.
/// Handles guest booking by:
/// 1. Registering the customer via the Customers API
/// 2. Calculating the price via the Pricing API
/// 3. Creating the reservation with the new customer ID
/// </summary>
public sealed class CreateGuestReservationCommandHandler(
    ICustomersService customersService,
    IReservationRepository reservations,
    IPricingService pricingService)
{
    public async Task<CreateGuestReservationResult> HandleAsync(
        CreateGuestReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Step 1: Register the customer via Customers API
        var registerCustomerDto = new RegisterCustomerDto(
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.DateOfBirth,
            command.Street,
            command.City,
            command.PostalCode,
            command.Country,
            command.LicenseNumber,
            command.LicenseIssueCountry,
            command.LicenseIssueDate,
            command.LicenseExpiryDate
        );

        var customerId = await customersService.RegisterCustomerAsync(
            registerCustomerDto,
            cancellationToken);

        // Step 2: Calculate price via Pricing API
        var priceCalculation = await pricingService.CalculatePriceAsync(
            command.CategoryCode,
            command.PickupDate,
            command.ReturnDate,
            command.PickupLocationCode,
            cancellationToken);

        var totalPrice = Money.Euro(priceCalculation.TotalPriceNet);

        // Step 3: Validate and create booking period
        var period = BookingPeriod.Of(command.PickupDate, command.ReturnDate);

        // Parse location codes
        var pickupLocationCode = LocationCode.Of(command.PickupLocationCode);
        var dropoffLocationCode = LocationCode.Of(command.DropoffLocationCode);

        // Step 4: Create the reservation aggregate with the new customer ID
        var reservation = Reservation.Create(
            command.VehicleId,
            customerId,
            period,
            pickupLocationCode,
            dropoffLocationCode,
            totalPrice
        );

        // Step 5: Save to repository
        await reservations.AddAsync(reservation, cancellationToken);
        await reservations.SaveChangesAsync(cancellationToken);

        // Step 6: Return result with both customer and reservation IDs
        return new CreateGuestReservationResult(
            CustomerId: customerId,
            ReservationId: reservation.Id.Value,
            TotalPriceNet: reservation.TotalPrice.NetAmount,
            TotalPriceVat: reservation.TotalPrice.VatAmount,
            TotalPriceGross: reservation.TotalPrice.GrossAmount,
            Currency: reservation.TotalPrice.Currency.Code
        );
    }
}

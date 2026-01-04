using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Commands;

/// <summary>
///     Handler for CreateGuestReservationCommand.
///     Handles guest booking by:
///     1. Registering the customer via the Customers API
///     2. Calculating the price via the Pricing API
///     3. Creating the reservation and persisting it to the database
/// </summary>
public sealed class CreateGuestReservationCommandHandler(
    IReservationRepository repository,
    ICustomersService customersService,
    IPricingService pricingService,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateGuestReservationCommand, CreateGuestReservationResult>
{
    public async Task<CreateGuestReservationResult> HandleAsync(
        CreateGuestReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        var (vehicleId, vehicleCategory, bookingPeriod, pickupLocationCode, dropoffLocationCode, name, email,
            phoneNumber, birthDate, address, driversLicense) = command;

        // Step 1: Register the customer via Customers API
        var registerCustomerDto = new RegisterCustomerDto(
            name.FirstName.Value,
            name.LastName.Value,
            email.Value,
            phoneNumber.Value,
            birthDate.Value,
            address.Street,
            address.City.Value,
            address.PostalCode.Value,
            address.Country,
            driversLicense.LicenseNumber,
            driversLicense.IssueCountry,
            driversLicense.IssueDate,
            driversLicense.ExpiryDate
        );

        var customerId = await customersService.RegisterCustomerAsync(registerCustomerDto, cancellationToken);

        // Step 2: Calculate price via Pricing API
        var priceCalculation = await pricingService.CalculatePriceAsync(
            vehicleCategory,
            bookingPeriod,
            pickupLocationCode,
            cancellationToken);

        var totalPrice = Money.Euro(priceCalculation.TotalPriceNet);

        // Step 3: Create reservation using static factory method
        var reservation = Reservation.Create(
            vehicleId,
            CustomerIdentifier.From(customerId),
            bookingPeriod,
            pickupLocationCode,
            dropoffLocationCode,
            totalPrice);

        // Persist to database
        await repository.AddAsync(reservation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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

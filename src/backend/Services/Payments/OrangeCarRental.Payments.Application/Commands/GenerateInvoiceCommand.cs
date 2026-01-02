using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

/// <summary>
///     Command to generate an invoice for a reservation.
/// </summary>
public sealed record GenerateInvoiceCommand(
    ReservationId ReservationId,
    CustomerId CustomerId,
    PersonName CustomerName,
    Street CustomerStreet,
    PostalCode CustomerPostalCode,
    City CustomerCity,
    Country CustomerCountry,
    VatId? CustomerVatId,
    string VehicleDescription,
    int RentalDays,
    Money DailyRate,
    DateOnly PickupDate,
    DateOnly ReturnDate,
    PaymentIdentifier? PaymentId = null) : ICommand<GenerateInvoiceResult>;

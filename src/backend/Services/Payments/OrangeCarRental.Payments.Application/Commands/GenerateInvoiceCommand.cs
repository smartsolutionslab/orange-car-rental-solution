namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

/// <summary>
///     Command to generate an invoice for a reservation.
/// </summary>
public sealed record GenerateInvoiceCommand(
    Guid ReservationId,
    Guid CustomerId,
    string CustomerName,
    string CustomerStreet,
    string CustomerPostalCode,
    string CustomerCity,
    string CustomerCountry,
    string? CustomerVatId,
    string VehicleDescription,
    int RentalDays,
    decimal DailyRateNet,
    DateOnly PickupDate,
    DateOnly ReturnDate,
    Guid? PaymentId = null);

namespace SmartSolutionsLab.OrangeCarRental.Payments.Api.Requests;

/// <summary>
///     Request DTO for generating an invoice.
/// </summary>
public sealed record GenerateInvoiceRequest(
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

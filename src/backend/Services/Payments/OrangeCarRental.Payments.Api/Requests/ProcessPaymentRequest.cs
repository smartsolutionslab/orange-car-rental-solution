namespace SmartSolutionsLab.OrangeCarRental.Payments.Api.Requests;

/// <summary>
///     Request DTO for processing a payment.
/// </summary>
public sealed record ProcessPaymentRequest(
    Guid ReservationId,
    Guid CustomerId,
    decimal Amount,
    string Currency,
    string PaymentMethod);

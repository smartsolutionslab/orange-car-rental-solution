using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;

public interface IPaymentService
{
    Task<(bool Success, string? TransactionId, string? ErrorMessage)> AuthorizePaymentAsync(
        decimal amount,
        string currency,
        PaymentMethod method,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string? ErrorMessage)> CapturePaymentAsync(
        string transactionId,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string? ErrorMessage)> RefundPaymentAsync(
        string transactionId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default);
}

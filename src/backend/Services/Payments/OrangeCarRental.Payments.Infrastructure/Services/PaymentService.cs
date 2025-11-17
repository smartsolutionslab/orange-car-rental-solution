using SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Services;

/// <summary>
///     Stub implementation of IPaymentService for payment processing.
///     In production, this would integrate with real payment providers like Stripe, PayPal, etc.
/// </summary>
public sealed class PaymentService : IPaymentService
{
    public Task<(bool Success, string? TransactionId, string? ErrorMessage)> AuthorizePaymentAsync(
        decimal amount,
        string currency,
        PaymentMethod method,
        CancellationToken cancellationToken = default)
    {
        // Stub implementation - always succeeds
        // In production, this would call real payment gateway APIs

        var transactionId = $"TXN-{Guid.CreateVersion7():N}";

        return Task.FromResult<(bool, string?, string?)>((true, transactionId, null));
    }

    public Task<(bool Success, string? ErrorMessage)> CapturePaymentAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        // Stub implementation - always succeeds
        // In production, this would capture the authorized payment

        return Task.FromResult<(bool, string?)>((true, null));
    }

    public Task<(bool Success, string? ErrorMessage)> RefundPaymentAsync(
        string transactionId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default)
    {
        // Stub implementation - always succeeds
        // In production, this would process refunds through the payment gateway

        return Task.FromResult<(bool, string?)>((true, null));
    }
}

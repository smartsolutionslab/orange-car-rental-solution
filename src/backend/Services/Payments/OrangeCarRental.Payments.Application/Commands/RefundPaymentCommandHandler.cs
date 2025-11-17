using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

public sealed class RefundPaymentCommandHandler(
    IPaymentRepository payments,
    IPaymentService paymentService)
    : ICommandHandler<RefundPaymentCommand, RefundPaymentResult>
{
    public async Task<RefundPaymentResult> HandleAsync(
        RefundPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        var paymentId = PaymentIdentifier.From(command.PaymentId);
        var payment = await payments.GetByIdAsync(paymentId, cancellationToken);

        if (payment == null)
        {
            throw new InvalidOperationException($"Payment with ID {command.PaymentId} not found");
        }

        // Validate payment can be refunded
        if (payment.Status != PaymentStatus.Captured)
        {
            throw new InvalidOperationException($"Payment with status {payment.Status} cannot be refunded. Only Captured payments can be refunded.");
        }

        if (string.IsNullOrEmpty(payment.TransactionId))
        {
            throw new InvalidOperationException("Payment does not have a transaction ID");
        }

        try
        {
            // Process refund with external provider
            var (success, errorMessage) = await paymentService.RefundPaymentAsync(
                payment.TransactionId,
                payment.Amount.GrossAmount,
                payment.Amount.Currency.Code,
                cancellationToken);

            if (success)
            {
                // Mark as refunded
                payment = payment.MarkAsRefunded();
                await payments.UpdateAsync(payment, cancellationToken);
                await payments.SaveChangesAsync(cancellationToken);

                return new RefundPaymentResult
                {
                    PaymentId = payment.Id.Value,
                    Status = payment.Status.ToString(),
                    RefundedAt = payment.RefundedAt!.Value
                };
            }
            else
            {
                throw new InvalidOperationException($"Refund failed: {errorMessage}");
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"Refund processing failed: {ex.Message}", ex);
        }
    }
}

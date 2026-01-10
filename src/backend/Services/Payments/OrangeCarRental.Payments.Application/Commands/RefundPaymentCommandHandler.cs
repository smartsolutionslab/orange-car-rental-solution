using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

public sealed class RefundPaymentCommandHandler(
    IPaymentsUnitOfWork unitOfWork,
    IPaymentService paymentService)
    : ICommandHandler<RefundPaymentCommand, RefundPaymentResult>
{
    public async Task<RefundPaymentResult> HandleAsync(
        RefundPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        var payments = unitOfWork.Payments;
        var payment = await payments.GetByIdAsync(command.PaymentId, cancellationToken)
            ?? throw new InvalidOperationException($"Payment with ID {command.PaymentId.Value} not found");

        // Validate payment can be refunded
        Ensure.That(payment.Status, nameof(payment.Status))
            .ThrowInvalidOperationIf(payment.Status != PaymentStatus.Captured, $"Payment with status {payment.Status} cannot be refunded. Only Captured payments can be refunded.")
            .ThrowInvalidOperationIf(!payment.TransactionId.HasValue, "Payment does not have a transaction ID");

        try
        {
            // Process refund with external provider
            var (success, errorMessage) = await paymentService.RefundPaymentAsync(
                payment.TransactionId!.Value.Value,
                payment.Amount.GrossAmount,
                payment.Amount.Currency.Code,
                cancellationToken);

            if (success)
            {
                // Mark as refunded
                payment = payment.MarkAsRefunded();
                await payments.UpdateAsync(payment, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

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

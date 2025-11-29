using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

public sealed class ProcessPaymentCommandHandler(
    IPaymentsUnitOfWork unitOfWork,
    IPaymentService paymentService)
    : ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult>
{
    public async Task<ProcessPaymentResult> HandleAsync(
        ProcessPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validate payment method
        Ensure.That(command.PaymentMethod, nameof(command.PaymentMethod))
            .ThrowIf(!Enum.TryParse<PaymentMethod>(command.PaymentMethod, ignoreCase: true, out var method),
                $"Invalid payment method: {command.PaymentMethod}");

        // Create payment aggregate
        var currency = Currency.From(command.Currency);
        var amount = Money.FromGross(command.Amount, 0.19m, currency);
        var payment = Payment.Create(
            command.ReservationId,
            command.CustomerId,
            amount,
            method);

        var payments = unitOfWork.Payments;
        await payments.AddAsync(payment, cancellationToken);

        try
        {
            // Authorize payment with external provider
            var (success, transactionId, errorMessage) = await paymentService.AuthorizePaymentAsync(
                amount.GrossAmount,
                currency.Code,
                method,
                cancellationToken);

            if (success && !string.IsNullOrEmpty(transactionId))
            {
                // Mark as authorized
                payment = payment.MarkAsAuthorized(transactionId);
                await payments.UpdateAsync(payment, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return new ProcessPaymentResult
                {
                    PaymentId = payment.Id.Value,
                    Status = payment.Status.ToString(),
                    TransactionId = transactionId,
                    ProcessedAt = payment.ProcessedAt!.Value
                };
            }
            else
            {
                // Mark as failed
                payment = payment.MarkAsFailed(errorMessage ?? "Payment authorization failed");
                await payments.UpdateAsync(payment, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                throw new InvalidOperationException($"Payment authorization failed: {errorMessage}");
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            // Mark as failed on exception
            payment = payment.MarkAsFailed(ex.Message);
            await payments.UpdateAsync(payment, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            throw new InvalidOperationException($"Payment processing failed: {ex.Message}", ex);
        }
    }
}

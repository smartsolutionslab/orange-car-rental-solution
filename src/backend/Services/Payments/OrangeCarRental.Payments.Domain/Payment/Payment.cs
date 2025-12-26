using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using TxId = SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects.TransactionId;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

/// <summary>
///     Payment aggregate root.
///     Represents a payment transaction for a reservation.
/// </summary>
public sealed class Payment : AggregateRoot<PaymentIdentifier>
{
    private Payment()
    {
        Amount = default;
    }

    /// <summary>
    ///     Referenced reservation ID.
    /// </summary>
    public Guid ReservationId { get; init; }

    /// <summary>
    ///     Referenced customer ID.
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    ///     Payment amount.
    /// </summary>
    public Money Amount { get; init; }

    /// <summary>
    ///     Payment method.
    /// </summary>
    public PaymentMethod Method { get; init; }

    /// <summary>
    ///     Payment status.
    /// </summary>
    public PaymentStatus Status { get; init; }

    /// <summary>
    ///     Transaction ID from payment provider.
    /// </summary>
    public TransactionId? TransactionId { get; init; }

    /// <summary>
    ///     Error message if payment failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    ///     Created timestamp.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Processed timestamp.
    /// </summary>
    public DateTime? ProcessedAt { get; init; }

    /// <summary>
    ///     Refunded timestamp.
    /// </summary>
    public DateTime? RefundedAt { get; init; }

    /// <summary>
    ///     Creates a new payment.
    /// </summary>
    public static Payment Create(
        Guid reservationId,
        Guid customerId,
        Money amount,
        PaymentMethod method)
    {
        return new Payment
        {
            Id = PaymentIdentifier.New(),
            ReservationId = reservationId,
            CustomerId = customerId,
            Amount = amount,
            Method = method,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    private Payment CreateMutatedCopy(
        PaymentStatus? status = null,
        TransactionId? transactionId = null,
        string? errorMessage = null,
        DateTime? processedAt = null,
        DateTime? refundedAt = null)
    {
        return new Payment
        {
            Id = Id,
            ReservationId = ReservationId,
            CustomerId = CustomerId,
            Amount = Amount,
            Method = Method,
            Status = status ?? Status,
            TransactionId = transactionId ?? TransactionId,
            ErrorMessage = errorMessage ?? ErrorMessage,
            CreatedAt = CreatedAt,
            ProcessedAt = processedAt ?? ProcessedAt,
            RefundedAt = refundedAt ?? RefundedAt
        };
    }

    /// <summary>
    ///     Marks the payment as authorized.
    /// </summary>
    public Payment MarkAsAuthorized(string transactionId)
    {
        return CreateMutatedCopy(
            status: PaymentStatus.Authorized,
            transactionId: TxId.Of(transactionId),
            processedAt: DateTime.UtcNow);
    }

    /// <summary>
    ///     Marks the payment as captured.
    /// </summary>
    public Payment MarkAsCaptured()
    {
        return CreateMutatedCopy(
            status: PaymentStatus.Captured,
            processedAt: DateTime.UtcNow);
    }

    /// <summary>
    ///     Marks the payment as failed.
    /// </summary>
    public Payment MarkAsFailed(string errorMessage)
    {
        return CreateMutatedCopy(
            status: PaymentStatus.Failed,
            errorMessage: errorMessage,
            processedAt: DateTime.UtcNow);
    }

    /// <summary>
    ///     Marks the payment as refunded.
    /// </summary>
    public Payment MarkAsRefunded()
    {
        return CreateMutatedCopy(
            status: PaymentStatus.Refunded,
            refundedAt: DateTime.UtcNow);
    }
}

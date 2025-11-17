using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

public sealed class Payment : AggregateRoot<PaymentIdentifier>
{
    private Payment()
    {
        Amount = default;
    }

    public Guid ReservationId { get; init; }
    public Guid CustomerId { get; init; }
    public Money Amount { get; init; }
    public PaymentMethod Method { get; init; }
    public PaymentStatus Status { get; init; }
    public string? TransactionId { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public DateTime? RefundedAt { get; init; }

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
        string? transactionId = null,
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

    public Payment MarkAsAuthorized(string transactionId)
    {
        return CreateMutatedCopy(
            status: PaymentStatus.Authorized,
            transactionId: transactionId,
            processedAt: DateTime.UtcNow);
    }

    public Payment MarkAsCaptured()
    {
        return CreateMutatedCopy(
            status: PaymentStatus.Captured,
            processedAt: DateTime.UtcNow);
    }

    public Payment MarkAsFailed(string errorMessage)
    {
        return CreateMutatedCopy(
            status: PaymentStatus.Failed,
            errorMessage: errorMessage,
            processedAt: DateTime.UtcNow);
    }

    public Payment MarkAsRefunded()
    {
        return CreateMutatedCopy(
            status: PaymentStatus.Refunded,
            refundedAt: DateTime.UtcNow);
    }
}

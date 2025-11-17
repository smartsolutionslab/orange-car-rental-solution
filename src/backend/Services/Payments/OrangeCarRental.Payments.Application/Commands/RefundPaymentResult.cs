namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

public sealed record RefundPaymentResult
{
    public required Guid PaymentId { get; init; }
    public required string Status { get; init; }
    public DateTime RefundedAt { get; init; }
}

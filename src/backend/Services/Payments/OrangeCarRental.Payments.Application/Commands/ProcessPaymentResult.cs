namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

public sealed record ProcessPaymentResult
{
    public required Guid PaymentId { get; init; }
    public required string Status { get; init; }
    public string? TransactionId { get; init; }
    public DateTime ProcessedAt { get; init; }
}

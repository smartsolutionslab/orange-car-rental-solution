using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

/// <summary>
///     Command to refund a payment.
/// </summary>
public sealed record RefundPaymentCommand(
    PaymentIdentifier PaymentId) : ICommand<RefundPaymentResult>;

using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

public sealed record RefundPaymentCommand(
    Guid PaymentId) : ICommand<RefundPaymentResult>;

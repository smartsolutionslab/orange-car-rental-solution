using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

public sealed record ProcessPaymentCommand(
    Guid ReservationId,
    Guid CustomerId,
    decimal Amount,
    string Currency,
    string PaymentMethod) : ICommand<ProcessPaymentResult>;

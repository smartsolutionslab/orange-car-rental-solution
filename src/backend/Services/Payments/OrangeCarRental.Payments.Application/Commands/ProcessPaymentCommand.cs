using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

/// <summary>
///     Command to process a payment for a reservation.
/// </summary>
public sealed record ProcessPaymentCommand(
    ReservationIdentifier ReservationIdentifier,
    CustomerIdentifier CustomerIdentifier,
    Money Amount,
    PaymentMethod PaymentMethod) : ICommand<ProcessPaymentResult>;

using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain;

/// <summary>
///     Unit of Work for the Payments bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public interface IPaymentsUnitOfWork : IUnitOfWork
{
    /// <summary>
    ///     Gets the payment repository.
    /// </summary>
    IPaymentRepository Payments { get; }
}

using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain;

/// <summary>
///     Unit of Work for the Customers bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public interface ICustomersUnitOfWork : IUnitOfWork
{
    /// <summary>
    ///     Gets the customer repository.
    /// </summary>
    ICustomerRepository Customers { get; }
}

using SmartSolutionsLab.OrangeCarRental.Customers.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence.Repositories;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;

/// <summary>
///     Unit of Work implementation for the Customers bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public sealed class CustomersUnitOfWork(CustomersDbContext context) : ICustomersUnitOfWork
{
    /// <inheritdoc />
    public ICustomerRepository Customers => field ??= new CustomerRepository(context);

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}

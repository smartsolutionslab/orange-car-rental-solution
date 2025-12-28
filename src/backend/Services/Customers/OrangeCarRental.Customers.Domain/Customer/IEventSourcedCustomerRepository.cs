namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
/// Repository interface for event-sourced Customer aggregate operations.
/// Separates event store operations from read model queries.
/// </summary>
public interface IEventSourcedCustomerRepository
{
    /// <summary>
    /// Loads a Customer aggregate from the event store.
    /// </summary>
    Task<Customer> LoadAsync(CustomerIdentifier id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a Customer aggregate's events to the event store.
    /// </summary>
    Task SaveAsync(Customer customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a customer exists in the event store.
    /// </summary>
    Task<bool> ExistsAsync(CustomerIdentifier id, CancellationToken cancellationToken = default);
}

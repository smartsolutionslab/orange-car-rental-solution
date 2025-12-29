using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.EventSourcing;

/// <summary>
/// Interface for publishing domain events from Customer aggregate.
/// Implementations can publish to message buses, event stores, etc.
/// </summary>
public interface ICustomerEventPublisher
{
    /// <summary>
    /// Publishes all uncommitted events from the customer aggregate.
    /// </summary>
    Task PublishEventsAsync(Customer customer, CancellationToken cancellationToken = default);
}

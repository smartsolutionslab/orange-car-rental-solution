using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.EventSourcing;

/// <summary>
/// No-op event publisher for when event publishing is not configured.
/// Can be replaced with implementations for Azure Service Bus, RabbitMQ, etc.
/// </summary>
public class NullCustomerEventPublisher : ICustomerEventPublisher
{
    public Task PublishEventsAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        // Events are available via customer.Changes but not published anywhere
        // Replace with actual implementation when message bus is configured
        return Task.CompletedTask;
    }
}

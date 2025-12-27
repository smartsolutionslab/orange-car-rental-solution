using Eventuous;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.EventSourcing;

/// <summary>
/// Event-sourced repository for Customer aggregates.
/// Uses Eventuous IEventReader/IEventWriter for event persistence and aggregate rehydration.
/// </summary>
public sealed class EventSourcedCustomerRepository : IEventSourcedCustomerRepository
{
    private readonly IEventReader _eventReader;
    private readonly IEventWriter _eventWriter;
    private const string StreamPrefix = "Customer-";

    public EventSourcedCustomerRepository(IEventReader eventReader, IEventWriter eventWriter)
    {
        _eventReader = eventReader;
        _eventWriter = eventWriter;
    }

    /// <summary>
    /// Loads a Customer aggregate from the event store by replaying its events.
    /// </summary>
    public async Task<Customer> LoadAsync(
        CustomerIdentifier id,
        CancellationToken cancellationToken = default)
    {
        var streamName = GetStreamName(id);
        var customer = new Customer();

        // Read events from the stream and fold them into the aggregate
        var events = await _eventReader.ReadStream(streamName, StreamReadPosition.Start, failIfNotFound: false, cancellationToken);
        customer.Load(events.Select(e => e.Payload).ToList());

        return customer;
    }

    /// <summary>
    /// Saves a Customer aggregate's uncommitted events to the event store.
    /// </summary>
    public async Task SaveAsync(
        Customer customer,
        CancellationToken cancellationToken = default)
    {
        if (customer.Changes.Count == 0)
            return;

        var streamName = GetStreamName(customer.Id);
        // For new aggregates, use ExpectedStreamVersion.NoStream
        // For existing aggregates, calculate from the current version
        var currentVersion = customer.CurrentVersion;
        var expectedVersion = currentVersion == -1
            ? ExpectedStreamVersion.NoStream
            : new ExpectedStreamVersion(currentVersion - customer.Changes.Count);

        await _eventWriter.Store(
            streamName,
            expectedVersion,
            customer.Changes.ToList(),
            null,
            cancellationToken);

        customer.ClearChanges();
    }

    /// <summary>
    /// Checks if a customer exists in the event store.
    /// </summary>
    public async Task<bool> ExistsAsync(
        CustomerIdentifier id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await LoadAsync(id, cancellationToken);
            return customer.State.HasBeenCreated;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static StreamName GetStreamName(CustomerIdentifier id) =>
        new($"{StreamPrefix}{id.Value}");
}

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

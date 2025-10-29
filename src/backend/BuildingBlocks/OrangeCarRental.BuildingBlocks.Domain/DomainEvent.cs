namespace OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
/// Base class for domain events.
/// Use past tense for event names (e.g., CustomerRegistered, not RegisterCustomer).
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public Guid EventId { get; init; }

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; init; }
}

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Base class for domain events.
///     Use past tense for event names (e.g., CustomerRegistered, not RegisterCustomer).
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <inheritdoc />
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}

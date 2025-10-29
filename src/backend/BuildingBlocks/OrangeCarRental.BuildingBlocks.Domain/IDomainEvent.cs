namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent something that happened in the domain that domain experts care about.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier for this event instance.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// When the event occurred (UTC).
    /// </summary>
    DateTime OccurredOnUtc { get; }
}

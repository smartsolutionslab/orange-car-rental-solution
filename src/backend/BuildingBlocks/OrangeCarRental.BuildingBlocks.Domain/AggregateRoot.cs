namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Base class for aggregate roots.
///     An aggregate is a cluster of domain objects that can be treated as a single unit.
///     The aggregate root is the entry point to the aggregate and enforces all invariants.
/// </summary>
/// <typeparam name="TId">The type of the aggregate's identifier (must be a value object).</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
{
    private readonly List<IDomainEvent> domainEvents = new();

    protected AggregateRoot()
    {
    }

    protected AggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>
    ///     Gets the domain events that have been raised by this aggregate.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    /// <summary>
    ///     Adds a domain event to be published.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent) => domainEvents.Add(domainEvent);

    /// <summary>
    ///     Clears all domain events.
    ///     Call this after the events have been published.
    /// </summary>
    public void ClearDomainEvents() => domainEvents.Clear();

    /// <summary>
    ///     Gets uncommitted events (for event sourcing).
    /// </summary>
    public IEnumerable<IDomainEvent> GetUncommittedEvents() => domainEvents.ToList();

    /// <summary>
    ///     Marks all events as committed (for event sourcing).
    /// </summary>
    public void MarkEventsAsCommitted() => domainEvents.Clear();
}

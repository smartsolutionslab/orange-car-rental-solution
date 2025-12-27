using Eventuous;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.EventSourcing;

/// <summary>
///     Base class for query models (aggregate state).
///     Extends Eventuous State with typed identifier support.
///     The query model serves as both:
///     - Internal state of the aggregate (built from events)
///     - Read model for queries (same structure)
/// </summary>
/// <typeparam name="TQueryModel">The self-referencing query model type.</typeparam>
/// <typeparam name="TId">The aggregate identifier type.</typeparam>
public abstract record QueryModel<TQueryModel, TId> : State<TQueryModel>
    where TQueryModel : QueryModel<TQueryModel, TId>, new()
    where TId : notnull
{
    /// <summary>
    ///     Gets the aggregate's typed identifier.
    /// </summary>
    public abstract TId Id { get; init; }

    /// <summary>
    ///     Indicates whether this aggregate has been created (has received its creation event).
    /// </summary>
    public abstract bool HasBeenCreated { get; }
}

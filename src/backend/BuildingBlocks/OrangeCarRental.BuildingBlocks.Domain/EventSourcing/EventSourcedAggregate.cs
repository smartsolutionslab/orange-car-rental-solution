using Eventuous;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.EventSourcing;

/// <summary>
///     Base class for event-sourced aggregates.
///     Combines Eventuous event sourcing with typed aggregate identifiers.
/// </summary>
/// <typeparam name="TQueryModel">The query model (state) type that handles event application.</typeparam>
/// <typeparam name="TId">The aggregate identifier type.</typeparam>
public abstract class EventSourcedAggregate<TQueryModel, TId> : Aggregate<TQueryModel>
    where TQueryModel : QueryModel<TQueryModel, TId>, new()
    where TId : notnull
{
    /// <summary>
    ///     Gets the aggregate's typed identifier.
    /// </summary>
    public TId Id => State.Id;

    /// <summary>
    ///     Ensures the aggregate exists (has been created).
    /// </summary>
    protected void EnsureExists()
    {
        if (!State.HasBeenCreated)
        {
            throw new InvalidOperationException($"{typeof(TQueryModel).Name.Replace("QueryModel", "")} does not exist");
        }
    }

    /// <summary>
    ///     Ensures the aggregate does not already exist.
    /// </summary>
    protected void EnsureDoesNotExist()
    {
        if (State.HasBeenCreated)
        {
            throw new InvalidOperationException($"{typeof(TQueryModel).Name.Replace("QueryModel", "")} already exists");
        }
    }
}

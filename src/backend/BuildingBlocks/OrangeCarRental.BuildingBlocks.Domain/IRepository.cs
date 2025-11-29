namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Full repository interface combining read and write operations for aggregate roots.
///     Use this when both read and write access is needed.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type.</typeparam>
/// <typeparam name="TIdentifier">The identifier type.</typeparam>
public interface IRepository<TAggregate, TIdentifier>
    : IReadRepository<TAggregate, TIdentifier>,
      IWriteRepository<TAggregate, TIdentifier>
    where TAggregate : AggregateRoot<TIdentifier>
    where TIdentifier : notnull
{
}

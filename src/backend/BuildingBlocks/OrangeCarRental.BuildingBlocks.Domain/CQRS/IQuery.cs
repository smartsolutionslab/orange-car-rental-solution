namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

/// <summary>
///     Marker interface for query objects in CQRS pattern.
///     Queries represent read operations that do not modify system state.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public interface IQuery<TResult>
{
}

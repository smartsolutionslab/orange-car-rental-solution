namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

/// <summary>
///     Marker interface for command objects in CQRS pattern.
///     Commands represent write operations that modify system state.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command.</typeparam>
public interface ICommand<TResult>
{
}

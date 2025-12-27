namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Marker interface for value objects in the domain.
///     Value objects are immutable objects that are defined by their attributes
///     rather than a unique identity. They implement value equality.
/// </summary>
/// <remarks>
///     Value objects should:
///     - Be immutable (readonly record struct or record)
///     - Implement value-based equality
///     - Contain validation logic in factory methods
///     - Not have an identity (no Id property)
///     - Be replaceable (updating means creating a new instance)
/// </remarks>
public interface IValueObject
{
}

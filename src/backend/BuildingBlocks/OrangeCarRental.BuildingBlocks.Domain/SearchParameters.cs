using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Base class for search parameters with pagination and sorting support.
/// </summary>
public abstract record SearchParameters(PagingInfo Paging, SortingInfo Sorting)
{
    /// <summary>
    ///     Validates the search parameters.
    /// </summary>
    public virtual void Validate()
    {
        // PagingInfo validates itself on creation
        // SortingInfo is always valid
        // Derived classes can add additional validation
    }
}

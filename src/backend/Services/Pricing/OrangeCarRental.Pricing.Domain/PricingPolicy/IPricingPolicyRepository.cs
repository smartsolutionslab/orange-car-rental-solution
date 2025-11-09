namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
///     Repository interface for pricing policies.
/// </summary>
public interface IPricingPolicyRepository
{
    /// <summary>
    ///     Gets a pricing policy by ID.
    /// </summary>
    /// <param name="id">The pricing policy identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The pricing policy.</returns>
    /// <exception cref="SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when the policy is not found.</exception>
    Task<PricingPolicy> GetByIdAsync(PricingPolicyIdentifier id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the active pricing policy for a vehicle category.
    /// </summary>
    /// <param name="categoryCode">The vehicle category code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The active pricing policy.</returns>
    /// <exception cref="SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when no active policy is found.</exception>
    Task<PricingPolicy> GetActivePolicyByCategoryAsync(CategoryCode categoryCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the active pricing policy for a vehicle category and location.
    /// </summary>
    /// <param name="categoryCode">The vehicle category code.</param>
    /// <param name="locationCode">The location code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The active pricing policy.</returns>
    /// <exception cref="SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when no active policy is found.</exception>
    Task<PricingPolicy> GetActivePolicyByCategoryAndLocationAsync(
        CategoryCode categoryCode,
        LocationCode locationCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all active pricing policies.
    /// </summary>
    Task<IReadOnlyCollection<PricingPolicy>> GetAllActivePoliciesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds a new pricing policy.
    /// </summary>
    Task AddAsync(PricingPolicy policy, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing pricing policy.
    /// </summary>
    Task UpdateAsync(PricingPolicy policy, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Saves all changes.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

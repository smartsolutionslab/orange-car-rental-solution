
namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
/// Repository interface for pricing policies.
/// </summary>
public interface IPricingPolicyRepository
{
    /// <summary>
    /// Gets a pricing policy by ID.
    /// </summary>
    Task<PricingPolicy?> GetByIdAsync(PricingPolicyId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the active pricing policy for a vehicle category.
    /// </summary>
    Task<PricingPolicy?> GetActivePolicyByCategoryAsync(CategoryCode categoryCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the active pricing policy for a vehicle category and location.
    /// </summary>
    Task<PricingPolicy?> GetActivePolicyByCategoryAndLocationAsync(
        CategoryCode categoryCode,
        LocationCode locationCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active pricing policies.
    /// </summary>
    Task<IReadOnlyCollection<PricingPolicy>> GetAllActivePoliciesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new pricing policy.
    /// </summary>
    Task AddAsync(PricingPolicy policy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing pricing policy.
    /// </summary>
    Task UpdateAsync(PricingPolicy policy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

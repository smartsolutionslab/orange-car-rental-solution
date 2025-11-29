using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain;

/// <summary>
///     Unit of Work for the Pricing bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public interface IPricingUnitOfWork : IUnitOfWork
{
    /// <summary>
    ///     Gets the pricing policy repository.
    /// </summary>
    IPricingPolicyRepository PricingPolicies { get; }
}

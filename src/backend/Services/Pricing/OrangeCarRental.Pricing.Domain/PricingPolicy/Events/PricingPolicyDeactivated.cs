using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy.Events;

/// <summary>
/// Domain event raised when a pricing policy is deactivated.
/// </summary>
public sealed record PricingPolicyDeactivated(
    PricingPolicyIdentifier PricingPolicyId,
    CategoryCode CategoryCode
) : DomainEvent;

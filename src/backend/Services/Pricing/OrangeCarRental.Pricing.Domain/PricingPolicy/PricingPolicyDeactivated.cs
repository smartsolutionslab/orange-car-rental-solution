using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

public sealed record PricingPolicyDeactivated(
    PricingPolicyId PricingPolicyId,
    CategoryCode CategoryCode
) : DomainEvent;

using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy.Events;

public sealed record PricingPolicyDeactivated(
    PricingPolicyIdentifier PricingPolicyId,
    CategoryCode CategoryCode
) : DomainEvent;

using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.Events;

public sealed record PricingPolicyDeactivated(
    PricingPolicyId PricingPolicyId,
    CategoryCode CategoryCode
) : DomainEvent;

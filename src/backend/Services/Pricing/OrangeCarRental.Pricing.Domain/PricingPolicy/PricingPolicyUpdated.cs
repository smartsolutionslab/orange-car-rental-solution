using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

public sealed record PricingPolicyUpdated(
    PricingPolicyId PricingPolicyId,
    CategoryCode CategoryCode,
    Money OldDailyRate,
    Money NewDailyRate
) : DomainEvent;

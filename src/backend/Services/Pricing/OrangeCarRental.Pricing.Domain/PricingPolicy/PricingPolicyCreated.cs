using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

public sealed record PricingPolicyCreated(
    PricingPolicyId PricingPolicyId,
    CategoryCode CategoryCode,
    Money DailyRate,
    DateTime EffectiveFrom
) : DomainEvent;

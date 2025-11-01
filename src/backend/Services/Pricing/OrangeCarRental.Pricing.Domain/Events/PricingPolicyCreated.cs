using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.Events;

public sealed record PricingPolicyCreated(
    PricingPolicyId PricingPolicyId,
    CategoryCode CategoryCode,
    Money DailyRate,
    DateTime EffectiveFrom
) : DomainEvent;

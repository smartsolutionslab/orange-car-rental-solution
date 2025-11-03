using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy.Events;

public sealed record PricingPolicyCreated(
    PricingPolicyIdentifier PricingPolicyId,
    CategoryCode CategoryCode,
    Money DailyRate,
    DateTime EffectiveFrom
) : DomainEvent;

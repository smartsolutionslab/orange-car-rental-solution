using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy.Events;

public sealed record PricingPolicyUpdated(
    PricingPolicyIdentifier PricingPolicyId,
    CategoryCode CategoryCode,
    Money OldDailyRate,
    Money NewDailyRate
) : DomainEvent;

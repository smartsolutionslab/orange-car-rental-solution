using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.Events;

public sealed record PricingPolicyUpdated(
    PricingPolicyId PricingPolicyId,
    CategoryCode CategoryCode,
    Money OldDailyRate,
    Money NewDailyRate
) : DomainEvent;

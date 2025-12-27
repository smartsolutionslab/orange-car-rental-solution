using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy.Events;

/// <summary>
///     Domain event raised when a pricing policy's daily rate is updated.
/// </summary>
public sealed record PricingPolicyUpdated(
    PricingPolicyIdentifier PricingPolicyId,
    CategoryCode CategoryCode,
    Money OldDailyRate,
    Money NewDailyRate
) : DomainEvent;

using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy.Events;

/// <summary>
///     Domain event raised when a new pricing policy is created for a vehicle category.
/// </summary>
public sealed record PricingPolicyCreated(
    PricingPolicyIdentifier PricingPolicyId,
    CategoryCode CategoryCode,
    Money DailyRate,
    DateTime EffectiveFrom
) : DomainEvent;

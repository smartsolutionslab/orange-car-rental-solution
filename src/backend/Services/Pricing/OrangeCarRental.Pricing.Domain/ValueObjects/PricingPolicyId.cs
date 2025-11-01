namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.ValueObjects;

/// <summary>
/// Unique identifier for a pricing policy.
/// </summary>
public readonly record struct PricingPolicyId
{
    public Guid Value { get; }

    private PricingPolicyId(Guid value) => Value = value;

    public static PricingPolicyId Of(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Pricing policy ID cannot be empty", nameof(value));

        return new PricingPolicyId(value);
    }

    public static PricingPolicyId New() => new(Guid.NewGuid());

    public static implicit operator Guid(PricingPolicyId id) => id.Value;

    public override string ToString() => Value.ToString();
}

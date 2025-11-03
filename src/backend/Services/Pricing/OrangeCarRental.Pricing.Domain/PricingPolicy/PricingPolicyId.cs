namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
/// Unique identifier for a pricing policy.
/// </summary>
public readonly record struct PricingPolicyId
{
    public Guid Value { get; }

    private PricingPolicyId(Guid value) => Value = value;

    public static PricingPolicyId Of(Guid value)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty, nameof(value));
        return new PricingPolicyId(value);
    }

    public static PricingPolicyId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException($"Invalid pricing policy ID format: {value}", nameof(value));
        }
        return Of(guid);
    }

    public static PricingPolicyId New() => new(Guid.CreateVersion7());

    public static implicit operator Guid(PricingPolicyId id) => id.Value;

    public override string ToString() => Value.ToString();
}

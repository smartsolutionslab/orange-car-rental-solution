namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
///     Unique identifier for a pricing policy.
/// </summary>
public readonly record struct PricingPolicyIdentifier
{
    private PricingPolicyIdentifier(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static PricingPolicyIdentifier Of(Guid value)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty, nameof(value));
        return new PricingPolicyIdentifier(value);
    }

    public static PricingPolicyIdentifier From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"Invalid pricing policy ID format: {value}", nameof(value));
        return Of(guid);
    }

    public static PricingPolicyIdentifier New() => new(Guid.CreateVersion7());

    public static implicit operator Guid(PricingPolicyIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}

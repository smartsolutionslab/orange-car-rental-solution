namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

/// <summary>
///     Postal code value object.
///     Represents a German postal code (5 digits, e.g., "10557").
/// </summary>
public readonly record struct PostalCode(string Value)
{
    public static PostalCode Empty => new(string.Empty);

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public static PostalCode Of(string postalCode)
    {
        // Postal code can be empty/optional
        var trimmed = postalCode?.Trim() ?? string.Empty;

        if (!string.IsNullOrEmpty(trimmed))
        {
            // German postal codes are 5 digits
            if (trimmed.Length != 5 || !trimmed.All(char.IsDigit))
                throw new ArgumentException("German postal code must be exactly 5 digits", nameof(postalCode));
        }

        return new PostalCode(trimmed);
    }

    public static implicit operator string(PostalCode postalCode) => postalCode.Value;

    public override string ToString() => Value;
}

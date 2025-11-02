namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for Customer aggregate.
/// Uses GUID v7 for time-ordered identifiers with better database performance.
/// </summary>
public readonly record struct CustomerId
{
    public Guid Value { get; }

    private CustomerId(Guid value) => Value = value;

    /// <summary>
    /// Creates a new unique customer identifier using GUID v7.
    /// </summary>
    public static CustomerId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Creates a customer identifier from an existing GUID.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <exception cref="ArgumentException">Thrown when the GUID is empty.</exception>
    public static CustomerId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Customer ID cannot be empty", nameof(value));
        }
        return new CustomerId(value);
    }

    /// <summary>
    /// Creates a customer identifier from a string representation of a GUID.
    /// </summary>
    /// <param name="value">The string representation of the GUID.</param>
    /// <exception cref="ArgumentException">Thrown when the string is not a valid GUID.</exception>
    public static CustomerId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException($"Invalid customer ID format: {value}", nameof(value));
        }

        return From(guid);
    }

    public static implicit operator Guid(CustomerId customerId) => customerId.Value;

    public override string ToString() => Value.ToString();
}

using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Strongly-typed identifier for Customer aggregate.
///     Uses GUID v7 for time-ordered identifiers with better database performance.
/// </summary>
public readonly record struct CustomerIdentifier(Guid Value) : IValueObject
{
    /// <summary>
    ///     Creates a new unique customer identifier using GUID v7.
    /// </summary>
    public static CustomerIdentifier New() => new(Guid.CreateVersion7());

    /// <summary>
    ///     Creates a customer identifier from an existing GUID.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <exception cref="ArgumentException">Thrown when the GUID is empty.</exception>
    public static CustomerIdentifier From(Guid value)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty, nameof(value));
        return new CustomerIdentifier(value);
    }

    /// <summary>
    ///     Creates a customer identifier from a string representation of a GUID.
    /// </summary>
    /// <param name="value">The string representation of the GUID.</param>
    /// <exception cref="ArgumentException">Thrown when the string is not a valid GUID.</exception>
    public static CustomerIdentifier From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"Invalid customer ID format: {value}", nameof(value));

        return From(guid);
    }

    public static implicit operator Guid(CustomerIdentifier customerIdentifier) => customerIdentifier.Value;

    public override string ToString() => Value.ToString();
}

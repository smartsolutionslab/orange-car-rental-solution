using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Represents a payment transaction identifier from an external payment provider.
///     Ensures consistent handling of transaction references across payment systems.
/// </summary>
public readonly record struct TransactionId : IValueObject
{
    /// <summary>
    ///     Gets the transaction ID value.
    /// </summary>
    public string Value { get; }

    private TransactionId(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a transaction ID from a string value.
    /// </summary>
    /// <param name="value">The transaction ID from the payment provider.</param>
    /// <exception cref="ArgumentException">Thrown when the value is empty or too long.</exception>
    public static TransactionId Of(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        return new TransactionId(value.Trim());
    }

    /// <summary>
    ///     Tries to create a transaction ID without throwing.
    /// </summary>
    public static bool TryCreate(string? value, out TransactionId? transactionId)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            transactionId = null;
            return true;
        }

        try
        {
            transactionId = Of(value);
            return true;
        }
        catch
        {
            transactionId = null;
            return false;
        }
    }

    public override string ToString() => Value;
}

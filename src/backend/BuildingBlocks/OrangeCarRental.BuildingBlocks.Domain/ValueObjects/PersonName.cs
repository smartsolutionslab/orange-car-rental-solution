using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Represents a person's full name.
///     Simple value object for cases where only the full name is needed,
///     without separate first/last name components.
/// </summary>
public readonly record struct PersonName : IValueObject
{
    /// <summary>
    ///     Gets the full name value.
    /// </summary>
    public string Value { get; }

    private PersonName(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a person name from a full name string.
    /// </summary>
    /// <param name="value">The full name.</param>
    /// <exception cref="ArgumentException">Thrown when the value is empty or too long.</exception>
    public static PersonName Of(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMinLength(2)
            .AndHasMaxLength(200);

        return new PersonName(value.Trim());
    }

    /// <summary>
    ///     Creates a person name from first and last name.
    /// </summary>
    public static PersonName Of(string firstName, string lastName)
    {
        Ensure.That(firstName, nameof(firstName)).IsNotNullOrWhiteSpace();
        Ensure.That(lastName, nameof(lastName)).IsNotNullOrWhiteSpace();

        return new PersonName($"{firstName.Trim()} {lastName.Trim()}");
    }

    public override string ToString() => Value;
}

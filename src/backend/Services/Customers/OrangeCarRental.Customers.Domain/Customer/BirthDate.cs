using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Birth date value object with German market validation.
///     Ensures valid date of birth for customer registration.
/// </summary>
public readonly record struct BirthDate(DateOnly Value) : IValueObject
{
    /// <summary>
    ///     Gets the customer's current age in years.
    /// </summary>
    public int Age
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - Value.Year;

            // Adjust if birthday hasn't occurred this year
            if (Value.AddYears(age) > today)
                age--;

            return age;
        }
    }

    /// <summary>
    ///     Creates a birth date value object from a DateOnly.
    /// </summary>
    /// <param name="value">The date of birth.</param>
    /// <exception cref="ArgumentException">Thrown when the date is invalid.</exception>
    public static BirthDate Of(DateOnly value)
    {
        ValidateBirthDate(value);
        return new BirthDate(value);
    }

    /// <summary>
    ///     Creates a birth date value object from year, month, and day.
    /// </summary>
    public static BirthDate Of(uint year, uint month, uint day)
    {
        var dateOnly = new DateOnly((int)year, (int)month, (int)day);
        return Of(dateOnly);
    }

    /// <summary>
    ///     Creates a birth date value object from a DateTime.
    /// </summary>
    public static BirthDate Of(DateTime dateTime)
    {
        return Of(DateOnly.FromDateTime(dateTime));
    }

    /// <summary>
    ///     Validates that the birth date is valid.
    /// </summary>
    private static void ValidateBirthDate(DateOnly value)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Date of birth cannot be in the future
        Ensure.That(value, nameof(value))
            .ThrowIf(value > today, "Date of birth cannot be in the future");

        // Sanity check - no one over 150 years old
        var age = today.Year - value.Year;
        if (value.AddYears(age) > today)
            age--;

        Ensure.That(value, nameof(value))
            .ThrowIf(age > 150, "Invalid date of birth - age cannot exceed 150 years");

        // Sanity check - minimum age of 0 (just born)
        Ensure.That(value, nameof(value))
            .ThrowIf(age < 0, "Invalid date of birth - age cannot be negative");
    }

    public static implicit operator DateOnly(BirthDate birthDate) => birthDate.Value;

    public override string ToString() => Value.ToString("yyyy-MM-dd");
}

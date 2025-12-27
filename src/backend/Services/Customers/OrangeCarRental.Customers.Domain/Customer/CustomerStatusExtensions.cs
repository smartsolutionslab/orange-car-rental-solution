using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Extension methods for parsing CustomerStatus from string.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class CustomerStatusExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for string to CustomerStatus parsing.
    /// </summary>
    extension(string value)
    {
        /// <summary>
        ///     Parses the string to a CustomerStatus enum value.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value is not a valid CustomerStatus.</exception>
        public CustomerStatus ParseCustomerStatus()
        {
            Ensure.That(value, nameof(value))
                .ThrowIf(!Enum.TryParse<CustomerStatus>(value, true, out _),
                    $"Invalid customer status: '{value}'. Valid values are: Active, Suspended, Blocked.");

            return Enum.Parse<CustomerStatus>(value, true);
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for nullable string to CustomerStatus parsing.
    /// </summary>
    extension(string? value)
    {
        /// <summary>
        ///     Tries to parse the string to a CustomerStatus enum value.
        /// </summary>
        /// <returns>The parsed CustomerStatus or null if parsing fails.</returns>
        public CustomerStatus? TryParseCustomerStatus()
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            Enum.TryParse<CustomerStatus>(value, true, out var parsedStatus);
            return parsedStatus;
        }
    }
}

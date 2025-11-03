using System.Text.RegularExpressions;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
/// Phone number value object with German phone format validation.
/// Supports formats: +49 XXX XXXXXXX, +49XXXXXXXXXX, 0XXX XXXXXXX
/// Stored in normalized format: +49XXXXXXXXXX
/// </summary>
public readonly record struct PhoneNumber
{
    // Matches German phone numbers in various formats
    private static readonly Regex GermanPhoneRegex = new(
        @"^(\+49|0049|0)[1-9]\d{1,14}$",
        RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    /// <summary>
    /// Creates a phone number value object from a string.
    /// </summary>
    /// <param name="value">The phone number in various formats.</param>
    /// <exception cref="ArgumentException">Thrown when the phone number is invalid.</exception>
    public static PhoneNumber Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty", nameof(value));

        // Remove common formatting characters
        var normalized = value.Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("/", "");

        // Convert German domestic format to international
        if (normalized.StartsWith("0") && !normalized.StartsWith("00"))
        {
            normalized = "+49" + normalized.Substring(1);
        }
        else if (normalized.StartsWith("0049"))
        {
            normalized = "+49" + normalized.Substring(4);
        }

        // Ensure it starts with +49
        if (!normalized.StartsWith("+49"))
        {
            throw new ArgumentException(
                "Phone number must be a German number starting with +49 or 0",
                nameof(value));
        }

        // Validate format
        if (!IsValidGermanPhone(normalized))
        {
            throw new ArgumentException(
                $"Invalid German phone number format: {value}. Expected format: +49XXXXXXXXXX",
                nameof(value));
        }

        // Validate length (German phone numbers: country code + 3-13 digits)
        if (normalized.Length < 6 || normalized.Length > 16)
        {
            throw new ArgumentException(
                "German phone number must be between 6 and 16 characters",
                nameof(value));
        }

        return new PhoneNumber(normalized);
    }

    private static bool IsValidGermanPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // Must start with +49
        if (!phone.StartsWith("+49"))
            return false;

        // Rest must be digits only
        var digitsOnly = phone.Substring(3);
        if (string.IsNullOrEmpty(digitsOnly) || !digitsOnly.All(char.IsDigit))
            return false;

        // First digit after country code cannot be 0
        if (digitsOnly[0] == '0')
            return false;

        return true;
    }

    /// <summary>
    /// Gets a formatted display version of the phone number.
    /// Example: +49 151 12345678
    /// </summary>
    public string FormattedValue
    {
        get
        {
            if (Value.Length <= 3)
                return Value;

            // Format as +49 XXX XXXXXXX
            var countryCode = "+49";
            var rest = Value.Substring(3);

            if (rest.Length <= 3)
                return $"{countryCode} {rest}";

            var areaCode = rest.Substring(0, Math.Min(3, rest.Length));
            var number = rest.Substring(Math.Min(3, rest.Length));

            return $"{countryCode} {areaCode} {number}";
        }
    }

    /// <summary>
    /// Creates an anonymized phone number for GDPR compliance.
    /// </summary>
    public static PhoneNumber Anonymized()
    {
        return new PhoneNumber("+490000000000");
    }

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;

    public override string ToString() => FormattedValue;
}

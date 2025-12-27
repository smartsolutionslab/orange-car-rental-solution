namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
///     Provides fluent validation extension methods for domain-specific German market rules.
/// </summary>
public static class EnsureDomainExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;string&gt; (German domain validations).
    /// </summary>
    extension(Ensurer<string> ensurer)
    {
        /// <summary>
        ///     Ensures the string is a valid German postal code (5 digits).
        /// </summary>
        public Ensurer<string> AndIsGermanPostalCode()
        {
            return ensurer
                .AndHasLengthBetween(5, 5)
                .AndSatisfies(
                    code => code.All(char.IsDigit),
                    "German postal code must be exactly 5 digits");
        }

        /// <summary>
        ///     Ensures the string is a valid German phone number format.
        ///     Supports formats: +49..., 0049..., or leading 0
        /// </summary>
        public Ensurer<string> AndIsGermanPhoneNumber()
        {
            const string pattern = @"^(\+49|0049|0)[1-9]\d{1,14}$";
            return ensurer.AndMatches(pattern, "valid German phone number");
        }

        /// <summary>
        ///     Ensures the string is a valid German driver's license number.
        ///     Format: Letters and digits, 5-11 characters
        /// </summary>
        public Ensurer<string> AndIsGermanDriversLicenseNumber()
        {
            return ensurer
                .AndHasLengthBetween(5, 11)
                .AndSatisfies(
                    license => license.All(char.IsLetterOrDigit),
                    "German driver's license must contain only letters and digits");
        }

        /// <summary>
        ///     Ensures the string contains only letters and spaces (for names).
        /// </summary>
        public Ensurer<string> AndContainsOnlyLettersAndSpaces()
        {
            return ensurer.AndSatisfies(
                value => value.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)),
                "Value must contain only letters and spaces");
        }

        /// <summary>
        ///     Ensures the string is a valid ISO country code (2 letters).
        /// </summary>
        public Ensurer<string> AndIsIsoCountryCode()
        {
            return ensurer
                .AndHasLengthBetween(2, 2)
                .AndSatisfies(
                    code => code.All(char.IsLetter) && code == code.ToUpperInvariant(),
                    "ISO country code must be exactly 2 uppercase letters");
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;DateOnly&gt;.
    /// </summary>
    extension(Ensurer<DateOnly> ensurer)
    {
        /// <summary>
        ///     Ensures the date is not in the future.
        /// </summary>
        public Ensurer<DateOnly> AndIsNotInFuture()
        {
            if (ensurer.Value > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("Date cannot be in the future.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the date is not in the past.
        /// </summary>
        public Ensurer<DateOnly> AndIsNotInPast()
        {
            if (ensurer.Value < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                throw new ArgumentException("Date cannot be in the past.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the date is today or in the future.
        /// </summary>
        public Ensurer<DateOnly> AndIsTodayOrFuture()
        {
            if (ensurer.Value < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                throw new ArgumentException("Date must be today or in the future.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the date is at least the specified number of days from now.
        /// </summary>
        public Ensurer<DateOnly> AndIsAtLeastDaysFromNow(int days)
        {
            var minDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(days));
            if (ensurer.Value < minDate)
                throw new ArgumentException(
                    $"Date must be at least {days} days from now.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the date is after the specified date.
        /// </summary>
        public Ensurer<DateOnly> AndIsAfter(DateOnly compareDate, string? compareName = null)
        {
            if (ensurer.Value <= compareDate)
            {
                var message = compareName != null
                    ? $"Date must be after {compareName}."
                    : $"Date must be after {compareDate:yyyy-MM-dd}.";
                throw new ArgumentException(message, ensurer.ParameterName);
            }

            return ensurer;
        }

        /// <summary>
        ///     Ensures the date is before the specified date.
        /// </summary>
        public Ensurer<DateOnly> AndIsBefore(DateOnly compareDate, string? compareName = null)
        {
            if (ensurer.Value >= compareDate)
            {
                var message = compareName != null
                    ? $"Date must be before {compareName}."
                    : $"Date must be before {compareDate:yyyy-MM-dd}.";
                throw new ArgumentException(message, ensurer.ParameterName);
            }

            return ensurer;
        }

        /// <summary>
        ///     Ensures the age (calculated from date of birth) meets minimum requirement.
        /// </summary>
        public Ensurer<DateOnly> AndMeetsMinimumAge(int minimumAge)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - ensurer.Value.Year;

            // Adjust if birthday hasn't occurred yet this year
            if (ensurer.Value > today.AddYears(-age))
                age--;

            if (age < minimumAge)
                throw new ArgumentException(
                    $"Person must be at least {minimumAge} years old (currently {age} years old).",
                    ensurer.ParameterName);

            return ensurer;
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;decimal&gt;.
    /// </summary>
    extension(Ensurer<decimal> ensurer)
    {
        /// <summary>
        ///     Ensures the decimal value is a valid price (positive, max 2 decimal places).
        /// </summary>
        public Ensurer<decimal> AndIsValidPrice()
        {
            if (ensurer.Value < 0)
                throw new ArgumentException("Price cannot be negative.", ensurer.ParameterName);

            if (decimal.Round(ensurer.Value, 2) != ensurer.Value)
                throw new ArgumentException("Price cannot have more than 2 decimal places.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the decimal value is within the specified range.
        /// </summary>
        public Ensurer<decimal> AndIsBetween(decimal min, decimal max)
        {
            if (ensurer.Value < min || ensurer.Value > max)
                throw new ArgumentException(
                    $"Value must be between {min} and {max}, but was {ensurer.Value}.",
                    ensurer.ParameterName);

            return ensurer;
        }
    }
}

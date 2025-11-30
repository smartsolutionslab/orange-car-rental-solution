using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
///     Provides fluent validation extension methods for string values.
/// </summary>
public static class EnsureStringExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;string&gt;.
    /// </summary>
    extension(Ensurer<string> ensurer)
    {
        /// <summary>
        ///     Ensures the string value is not null.
        /// </summary>
        [return: NotNull]
        public Ensurer<string> IsNotNull()
        {
            if (ensurer.Value is null)
                throw new ArgumentNullException(ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string value is not null or empty.
        /// </summary>
        [return: NotNull]
        public Ensurer<string> IsNotNullOrEmpty()
        {
            if (string.IsNullOrEmpty(ensurer.Value))
                throw new ArgumentException("String cannot be null or empty.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string value is not null, empty, or whitespace.
        /// </summary>
        [return: NotNull]
        public Ensurer<string> IsNotNullOrWhiteSpace()
        {
            if (string.IsNullOrWhiteSpace(ensurer.Value))
                throw new ArgumentException("String cannot be null, empty, or whitespace.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string has a minimum length.
        /// </summary>
        public Ensurer<string> AndHasMinLength(int minLength)
        {
            if (ensurer.Value?.Length < minLength)
                throw new ArgumentException(
                    $"String must have at least {minLength} characters but has {ensurer.Value?.Length ?? 0}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string has a maximum length.
        /// </summary>
        public Ensurer<string> AndHasMaxLength(int maxLength)
        {
            if (ensurer.Value?.Length > maxLength)
                throw new ArgumentException(
                    $"String must have at most {maxLength} characters but has {ensurer.Value.Length}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string length is within the specified range.
        /// </summary>
        public Ensurer<string> AndHasLengthBetween(int minLength, int maxLength)
        {
            var length = ensurer.Value?.Length ?? 0;
            if (length < minLength || length > maxLength)
                throw new ArgumentException(
                    $"String length must be between {minLength} and {maxLength} characters but has {length}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string is longer than the specified length.
        /// </summary>
        public Ensurer<string> AndIsLongerThan(int length)
        {
            if (ensurer.Value?.Length <= length)
                throw new ArgumentException(
                    $"String must be longer than {length} characters but has {ensurer.Value?.Length ?? 0}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string is shorter than the specified length.
        /// </summary>
        public Ensurer<string> AndIsShorterThan(int length)
        {
            if (ensurer.Value?.Length >= length)
                throw new ArgumentException(
                    $"String must be shorter than {length} characters but has {ensurer.Value.Length}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string matches the specified regular expression pattern.
        /// </summary>
        public Ensurer<string> AndMatches(string pattern, string? patternDescription = null)
        {
            if (ensurer.Value is null || !Regex.IsMatch(ensurer.Value, pattern))
            {
                var description = patternDescription ?? pattern;
                throw new ArgumentException($"String does not match the required pattern: {description}.",
                    ensurer.ParameterName);
            }

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string does not match the specified regular expression pattern.
        /// </summary>
        public Ensurer<string> AndDoesNotMatch(string pattern, string? patternDescription = null)
        {
            if (ensurer.Value is not null && Regex.IsMatch(ensurer.Value, pattern))
            {
                var description = patternDescription ?? pattern;
                throw new ArgumentException($"String must not match the pattern: {description}.", ensurer.ParameterName);
            }

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string contains the specified substring.
        /// </summary>
        public Ensurer<string> AndContains(string substring)
        {
            if (ensurer.Value is null || !ensurer.Value.Contains(substring, StringComparison.Ordinal))
                throw new ArgumentException($"String must contain '{substring}'.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string does not contain the specified substring.
        /// </summary>
        public Ensurer<string> AndDoesNotContain(string substring)
        {
            if (ensurer.Value is not null && ensurer.Value.Contains(substring, StringComparison.Ordinal))
                throw new ArgumentException($"String must not contain '{substring}'.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string starts with the specified prefix.
        /// </summary>
        public Ensurer<string> AndStartsWith(string prefix)
        {
            if (ensurer.Value is null || !ensurer.Value.StartsWith(prefix, StringComparison.Ordinal))
                throw new ArgumentException($"String must start with '{prefix}'.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string ends with the specified suffix.
        /// </summary>
        public Ensurer<string> AndEndsWith(string suffix)
        {
            if (ensurer.Value is null || !ensurer.Value.EndsWith(suffix, StringComparison.Ordinal))
                throw new ArgumentException($"String must end with '{suffix}'.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string equals the specified value.
        /// </summary>
        public Ensurer<string> AndEquals(string expected, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (ensurer.Value is null || !ensurer.Value.Equals(expected, comparisonType))
                throw new ArgumentException($"String must equal '{expected}'.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the string is a valid email address format.
        /// </summary>
        public Ensurer<string> AndIsValidEmail()
        {
            const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return ensurer.AndMatches(emailPattern, "valid email address");
        }

        /// <summary>
        ///     Ensures the string is a valid URL format.
        /// </summary>
        public Ensurer<string> AndIsValidUrl()
        {
            if (ensurer.Value is null || !Uri.TryCreate(ensurer.Value, UriKind.Absolute, out _))
                throw new ArgumentException("String must be a valid URL.", ensurer.ParameterName);

            return ensurer;
        }
    }
}
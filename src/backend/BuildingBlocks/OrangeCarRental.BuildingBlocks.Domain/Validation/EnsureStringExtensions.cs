using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
/// Provides fluent validation extension methods for string values.
/// </summary>
public static class EnsureStringExtensions
{
    /// <summary>
    /// Ensures the string value is not null.
    /// </summary>
    public static Ensurer<string> IsNotNull([NotNull] this Ensurer<string> ensurer)
    {
        if (ensurer.Value is null)
            throw new ArgumentNullException(ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string value is not null or empty.
    /// </summary>
    public static Ensurer<string> IsNotNullOrEmpty([NotNull] this Ensurer<string> ensurer)
    {
        if (string.IsNullOrEmpty(ensurer.Value))
            throw new ArgumentException($"String cannot be null or empty.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string value is not null, empty, or whitespace.
    /// </summary>
    public static Ensurer<string> IsNotNullOrWhiteSpace([NotNull] this Ensurer<string> ensurer)
    {
        if (string.IsNullOrWhiteSpace(ensurer.Value))
            throw new ArgumentException($"String cannot be null, empty, or whitespace.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string has a minimum length.
    /// </summary>
    public static Ensurer<string> AndHasMinLength(this Ensurer<string> ensurer, int minLength)
    {
        if (ensurer.Value?.Length < minLength)
            throw new ArgumentException($"String must have at least {minLength} characters but has {ensurer.Value?.Length ?? 0}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string has a maximum length.
    /// </summary>
    public static Ensurer<string> AndHasMaxLength(this Ensurer<string> ensurer, int maxLength)
    {
        if (ensurer.Value?.Length > maxLength)
            throw new ArgumentException($"String must have at most {maxLength} characters but has {ensurer.Value.Length}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string length is within the specified range.
    /// </summary>
    public static Ensurer<string> AndHasLengthBetween(this Ensurer<string> ensurer, int minLength, int maxLength)
    {
        var length = ensurer.Value?.Length ?? 0;
        if (length < minLength || length > maxLength)
            throw new ArgumentException($"String length must be between {minLength} and {maxLength} characters but has {length}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string is longer than the specified length.
    /// </summary>
    public static Ensurer<string> AndIsLongerThan(this Ensurer<string> ensurer, int length)
    {
        if (ensurer.Value?.Length <= length)
            throw new ArgumentException($"String must be longer than {length} characters but has {ensurer.Value?.Length ?? 0}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string is shorter than the specified length.
    /// </summary>
    public static Ensurer<string> AndIsShorterThan(this Ensurer<string> ensurer, int length)
    {
        if (ensurer.Value?.Length >= length)
            throw new ArgumentException($"String must be shorter than {length} characters but has {ensurer.Value.Length}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string matches the specified regular expression pattern.
    /// </summary>
    public static Ensurer<string> AndMatches(this Ensurer<string> ensurer, string pattern, string? patternDescription = null)
    {
        if (ensurer.Value is null || !Regex.IsMatch(ensurer.Value, pattern))
        {
            var description = patternDescription ?? pattern;
            throw new ArgumentException($"String does not match the required pattern: {description}.", ensurer.ParameterName);
        }

        return ensurer;
    }

    /// <summary>
    /// Ensures the string does not match the specified regular expression pattern.
    /// </summary>
    public static Ensurer<string> AndDoesNotMatch(this Ensurer<string> ensurer, string pattern, string? patternDescription = null)
    {
        if (ensurer.Value is not null && Regex.IsMatch(ensurer.Value, pattern))
        {
            var description = patternDescription ?? pattern;
            throw new ArgumentException($"String must not match the pattern: {description}.", ensurer.ParameterName);
        }

        return ensurer;
    }

    /// <summary>
    /// Ensures the string contains the specified substring.
    /// </summary>
    public static Ensurer<string> AndContains(this Ensurer<string> ensurer, string substring)
    {
        if (ensurer.Value is null || !ensurer.Value.Contains(substring, StringComparison.Ordinal))
            throw new ArgumentException($"String must contain '{substring}'.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string does not contain the specified substring.
    /// </summary>
    public static Ensurer<string> AndDoesNotContain(this Ensurer<string> ensurer, string substring)
    {
        if (ensurer.Value is not null && ensurer.Value.Contains(substring, StringComparison.Ordinal))
            throw new ArgumentException($"String must not contain '{substring}'.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string starts with the specified prefix.
    /// </summary>
    public static Ensurer<string> AndStartsWith(this Ensurer<string> ensurer, string prefix)
    {
        if (ensurer.Value is null || !ensurer.Value.StartsWith(prefix, StringComparison.Ordinal))
            throw new ArgumentException($"String must start with '{prefix}'.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string ends with the specified suffix.
    /// </summary>
    public static Ensurer<string> AndEndsWith(this Ensurer<string> ensurer, string suffix)
    {
        if (ensurer.Value is null || !ensurer.Value.EndsWith(suffix, StringComparison.Ordinal))
            throw new ArgumentException($"String must end with '{suffix}'.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string equals the specified value.
    /// </summary>
    public static Ensurer<string> AndEquals(this Ensurer<string> ensurer, string expected, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (ensurer.Value is null || !ensurer.Value.Equals(expected, comparisonType))
            throw new ArgumentException($"String must equal '{expected}'.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the string is a valid email address format.
    /// </summary>
    public static Ensurer<string> AndIsValidEmail(this Ensurer<string> ensurer)
    {
        const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return ensurer.AndMatches(emailPattern, "valid email address");
    }

    /// <summary>
    /// Ensures the string is a valid URL format.
    /// </summary>
    public static Ensurer<string> AndIsValidUrl(this Ensurer<string> ensurer)
    {
        if (ensurer.Value is null || !Uri.TryCreate(ensurer.Value, UriKind.Absolute, out _))
            throw new ArgumentException("String must be a valid URL.", ensurer.ParameterName);

        return ensurer;
    }
}

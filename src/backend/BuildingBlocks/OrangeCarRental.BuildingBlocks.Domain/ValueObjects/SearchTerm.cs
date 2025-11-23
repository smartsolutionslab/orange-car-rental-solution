using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Search term value object for text-based search queries.
///     Represents a user-provided search string with validation and normalization.
/// </summary>
/// <param name="Value">The search term value.</param>
public readonly record struct SearchTerm(string Value) : IValueObject
{
    /// <summary>
    ///     Creates a search term value object with validation.
    /// </summary>
    /// <param name="searchTerm">The search term string.</param>
    /// <returns>A validated SearchTerm value object.</returns>
    /// <exception cref="ArgumentException">Thrown when the search term is invalid.</exception>
    public static SearchTerm Of(string searchTerm)
    {
        var trimmed = searchTerm?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(searchTerm))
            .IsNotNullOrWhiteSpace()
            .AndHasMinLength(2)
            .AndHasMaxLength(200);

        return new SearchTerm(trimmed);
    }

    public static SearchTerm? TryParse(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return null;

        return Of(searchTerm);
    }

    /// <summary>
    ///     Checks if the search term contains a specific substring (case-insensitive).
    /// </summary>
    public bool Contains(string substring) =>
        Value.Contains(substring, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Checks if a given string matches the search term (case-insensitive).
    /// </summary>
    public bool Matches(string text) =>
        text?.Contains(Value, StringComparison.OrdinalIgnoreCase) ?? false;

    public static implicit operator string(SearchTerm searchTerm) => searchTerm.Value;

    public override string ToString() => Value;
}

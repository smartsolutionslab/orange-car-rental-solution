using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Image URL value object.
///     Validates and normalizes image URLs for vehicle images.
/// </summary>
/// <param name="Value">The validated and trimmed URL value.</param>
public readonly record struct ImageUrl(string Value) : IValueObject
{
    private const int MaxLength = 500;
    private static readonly string[] AllowedSchemes = ["http", "https"];
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"];

    public static ImageUrl From(string value)
    {
        var trimmed = value?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(MaxLength)
            .AndSatisfies(
                v => Uri.TryCreate(v, UriKind.Absolute, out _),
                $"Invalid URL format: '{value}'.")
            .AndSatisfies(
                v => Uri.TryCreate(v, UriKind.Absolute, out var u) && AllowedSchemes.Contains(u.Scheme.ToLowerInvariant()),
                "URL must use HTTP or HTTPS scheme.");

        return new ImageUrl(trimmed);
    }

    public static ImageUrl? FromNullable(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        return From(value);
    }

    public static bool TryParse(string? value, out ImageUrl result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
            return false;

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
            return false;

        if (!AllowedSchemes.Contains(uri.Scheme.ToLowerInvariant()))
            return false;

        result = new ImageUrl(trimmed);
        return true;
    }

    /// <summary>
    ///     Checks if the URL points to an image file based on extension.
    /// </summary>
    public bool HasImageExtension()
    {
        if (!Uri.TryCreate(Value, UriKind.Absolute, out var uri))
            return false;

        var path = uri.AbsolutePath.ToLowerInvariant();
        return AllowedImageExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }

    public static implicit operator string(ImageUrl imageUrl) => imageUrl.Value;

    public override string ToString() => Value;
}

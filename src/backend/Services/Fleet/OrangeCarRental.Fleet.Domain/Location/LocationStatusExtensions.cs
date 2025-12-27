using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

/// <summary>
///     Extension methods for parsing LocationStatus from string.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class LocationStatusExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for string to LocationStatus parsing.
    /// </summary>
    extension(string value)
    {
        /// <summary>
        ///     Parses the string to a LocationStatus enum value.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value is not a valid LocationStatus.</exception>
        public LocationStatus ParseLocationStatus()
        {
            Ensure.That(value, nameof(value))
                .ThrowIf(!Enum.TryParse<LocationStatus>(value, true, out _),
                    $"Invalid location status: '{value}'. Valid values are: Active, Closed, UnderMaintenance, Inactive.");

            return Enum.Parse<LocationStatus>(value, true);
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for nullable string to LocationStatus parsing.
    /// </summary>
    extension(string? value)
    {
        /// <summary>
        ///     Tries to parse the string to a LocationStatus enum value.
        /// </summary>
        /// <returns>The parsed LocationStatus or null if parsing fails.</returns>
        public LocationStatus? TryParseLocationStatus()
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            Enum.TryParse<LocationStatus>(value, true, out var parsedStatus);
            return parsedStatus;
        }
    }
}

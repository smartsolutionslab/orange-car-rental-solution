using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Extension methods for parsing VehicleStatus from string.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class VehicleStatusExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for string to VehicleStatus parsing.
    /// </summary>
    extension(string value)
    {
        /// <summary>
        ///     Parses the string to a VehicleStatus enum value.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value is not a valid VehicleStatus.</exception>
        public VehicleStatus ParseVehicleStatus()
        {
            Ensure.That(value, nameof(value))
                .ThrowIf(!Enum.TryParse<VehicleStatus>(value, true, out _),
                    $"Invalid vehicle status: '{value}'. Valid values are: Available, Rented, Maintenance, OutOfService, Reserved.");

            return Enum.Parse<VehicleStatus>(value, true);
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for nullable string to VehicleStatus parsing.
    /// </summary>
    extension(string? value)
    {
        /// <summary>
        ///     Tries to parse the string to a VehicleStatus enum value.
        /// </summary>
        /// <returns>The parsed VehicleStatus or null if parsing fails.</returns>
        public VehicleStatus? TryParseVehicleStatus()
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            Enum.TryParse<VehicleStatus>(value, true, out var parsedStatus);
            return parsedStatus;
        }
    }
}

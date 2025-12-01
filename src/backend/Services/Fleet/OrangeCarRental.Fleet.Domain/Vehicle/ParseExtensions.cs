using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Extension methods for parsing vehicle-related enums from string.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class ParseExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for string to TransmissionType/FuelType parsing.
    /// </summary>
    extension(string value)
    {
        /// <summary>
        ///     Parses the string to a TransmissionType enum value.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value is null, empty, or not a valid TransmissionType.</exception>
        public TransmissionType ParseTransmissionType()
        {
            Ensure.That(value, nameof(value))
                .IsNotNullOrWhiteSpace();
            return Enum.Parse<TransmissionType>(value, true);
        }

        /// <summary>
        ///     Parses the string to a FuelType enum value.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value is null, empty, or not a valid FuelType.</exception>
        public FuelType ParseFuelType()
        {
            Ensure.That(value, nameof(value))
                .IsNotNullOrWhiteSpace();
            return Enum.Parse<FuelType>(value, true);
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for nullable string to TransmissionType/FuelType parsing.
    /// </summary>
    extension(string? value)
    {
        /// <summary>
        ///     Tries to parse the string to a TransmissionType enum value.
        /// </summary>
        /// <returns>The parsed TransmissionType or null if parsing fails.</returns>
        public TransmissionType? TryParseTransmissionType()
        {
            if (Enum.TryParse(value, true, out TransmissionType parsedTransmissionType))
                return parsedTransmissionType;

            return null;
        }

        /// <summary>
        ///     Tries to parse the string to a FuelType enum value.
        /// </summary>
        /// <returns>The parsed FuelType or null if parsing fails.</returns>
        public FuelType? TryParseFuelType()
        {
            if (Enum.TryParse(value, true, out FuelType parsedFuelType))
                return parsedFuelType;

            return null;
        }
    }
}

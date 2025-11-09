using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

public static class ParseExtensions
{
    public static TransmissionType ParseTransmissionType(this string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace();
        return Enum.Parse<TransmissionType>(value, true);
    }

    public static TransmissionType? TryParse(this string? value)
    {
        if (Enum.TryParse(value, true, out TransmissionType parsedTransmissionType)) return parsedTransmissionType;

        return null;
    }

    public static FuelType ParseFuelType(this string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace();
        return Enum.Parse<FuelType>(value, true);
    }

    public static FuelType? TryParseFuelType(this string? value)
    {
        if (Enum.TryParse(value, true, out FuelType parsedFuelType)) return parsedFuelType;

        return null;
    }
}

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Enums;

public static class ParseExtensions
{
    private static TransmissionType? ParseTransmissionType(string value)
    {
        if(Enum.TryParse(value, true, out TransmissionType parsedTransmissionType))
        {
            return parsedTransmissionType;
        }

        return null;
    }

    private static FuelType? ParseFuelType(string value)
    {
        if (Enum.TryParse(value, true, out FuelType parsedFuelType))
        {
            return parsedFuelType;
        }

        return null;
    }
}

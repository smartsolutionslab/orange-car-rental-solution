namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

public static class VehicleStatusExtensions
{
    public static VehicleStatus Parse(this string value)
    {
        if (!Enum.TryParse<VehicleStatus>(value, true, out var newStatus))
        {
            throw new ArgumentException($"Invalid customer status: '{value}'. Valid values are: Available, Rented, Maintenance, OutOfService, Reserved.",
                nameof(value));
        }

        return newStatus;
    }

    public static VehicleStatus? TryParse(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        Enum.TryParse<VehicleStatus>(value, true, out var parsedStatus);
        return parsedStatus;
    }
}

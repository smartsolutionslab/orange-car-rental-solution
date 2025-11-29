using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

public static class VehicleStatusExtensions
{
    public static VehicleStatus Parse(this string value)
    {
        Ensure.That(value, nameof(value))
            .ThrowIf(!Enum.TryParse<VehicleStatus>(value, true, out var newStatus),
                $"Invalid vehicle status: '{value}'. Valid values are: Available, Rented, Maintenance, OutOfService, Reserved.");

        return Enum.Parse<VehicleStatus>(value, true);
    }

    public static VehicleStatus? TryParse(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        Enum.TryParse<VehicleStatus>(value, true, out var parsedStatus);
        return parsedStatus;
    }
}

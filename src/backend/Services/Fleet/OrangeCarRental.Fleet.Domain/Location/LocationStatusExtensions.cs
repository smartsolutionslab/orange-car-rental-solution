using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

public static class LocationStatusExtensions
{
    public static LocationStatus ParseLocationStatus(this string value)
    {
        Ensure.That(value, nameof(value))
            .ThrowIf(!Enum.TryParse<LocationStatus>(value, true, out var newStatus),
                $"Invalid location status: '{value}'. Valid values are: Active, Closed, UnderMaintenance, Inactive.");

        return Enum.Parse<LocationStatus>(value, true);
    }

    public static LocationStatus? TryParseLocationStatus(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        Enum.TryParse<LocationStatus>(value, true, out var parsedStatus);
        return parsedStatus;
    }
}

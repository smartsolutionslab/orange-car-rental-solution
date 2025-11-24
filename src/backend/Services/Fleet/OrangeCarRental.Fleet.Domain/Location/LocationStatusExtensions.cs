namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

public static class LocationStatusExtensions
{
    public static LocationStatus ParseLocationStatus(this string value)
    {
        if (!Enum.TryParse<LocationStatus>(value, true, out var newStatus))
        {
            throw new ArgumentException($"Invalid customer status: '{value}'. Valid values are: Active, Closed, UnderMaintenance, Inactive.",
                nameof(value));
        }

        return newStatus;
    }

    public static LocationStatus? TryParseLocationStatus(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        Enum.TryParse<LocationStatus>(value, true, out var parsedStatus);
        return parsedStatus;
    }
}

using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Geographic coordinates (latitude/longitude) for a location.
/// </summary>
public readonly record struct GeoCoordinates(decimal Latitude, decimal Longitude) : IValueObject
{
    public static GeoCoordinates Of(decimal latitude, decimal longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        return new GeoCoordinates(latitude, longitude);
    }

    public override string ToString() => $"{Latitude}, {Longitude}";
}

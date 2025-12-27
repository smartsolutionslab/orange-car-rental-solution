using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Geographic coordinates (latitude/longitude) for a location.
/// </summary>
public readonly record struct GeoCoordinates(decimal Latitude, decimal Longitude) : IValueObject
{
    public static GeoCoordinates Of(decimal latitude, decimal longitude)
    {
        Ensure.That(latitude, nameof(latitude))
            .IsGreaterThanOrEqual(-90m)
            .AndIsLessThanOrEqual(90m);

        Ensure.That(longitude, nameof(longitude))
            .IsGreaterThanOrEqual(-180m)
            .AndIsLessThanOrEqual(180m);

        return new GeoCoordinates(latitude, longitude);
    }

    public override string ToString() => $"{Latitude}, {Longitude}";
}

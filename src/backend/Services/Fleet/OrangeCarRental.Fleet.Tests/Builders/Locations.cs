using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Builders;

public static class Locations
{
    public static LocationCode BerlinHauptbahnhof => LocationCode.From(TestLocations.BerlinHbf);
    public static LocationCode MunichAirport => LocationCode.From(TestLocations.MunichAirport);
    public static LocationCode FrankfurtAirport => LocationCode.From(TestLocations.FrankfurtAirport);
    public static LocationCode HamburgHbf => LocationCode.From(TestLocations.HamburgHbf);
}

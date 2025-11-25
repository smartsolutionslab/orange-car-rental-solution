using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Builders;

public static class Locations
{
    public static LocationCode BerlinHauptbahnhof => LocationCode.Of("BER-HBF");
    public static LocationCode MunichAirport => LocationCode.Of("MUC-FLG");
}

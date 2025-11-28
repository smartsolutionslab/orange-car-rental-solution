using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Builders;

public static class Locations
{
    public static LocationCode BerlinHauptbahnhof => LocationCode.From("BER-HBF");
    public static LocationCode MunichAirport => LocationCode.From("MUC-FLG");
}

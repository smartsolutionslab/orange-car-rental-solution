using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;

/// <summary>
/// Helper methods for creating Money values in tests.
/// Uses German VAT rate (19%) by default.
/// </summary>
public static class TestMoney
{
    /// <summary>
    /// German standard VAT rate.
    /// </summary>
    public const decimal GermanVatRate = 0.19m;

    /// <summary>
    /// German reduced VAT rate (for certain goods/services).
    /// </summary>
    public const decimal GermanReducedVatRate = 0.07m;

    /// <summary>
    /// Creates Money in EUR from a gross amount with 19% VAT.
    /// </summary>
    public static Money EuroGross(decimal grossAmount)
        => Money.FromGross(grossAmount, GermanVatRate, Currency.EUR);

    /// <summary>
    /// Creates Money in EUR from a net amount with 19% VAT.
    /// </summary>
    public static Money EuroNet(decimal netAmount)
        => Money.Of(netAmount, GermanVatRate, Currency.EUR);

    /// <summary>
    /// Creates zero Money in EUR.
    /// </summary>
    public static Money Zero => Money.Of(0m, GermanVatRate, Currency.EUR);

    /// <summary>
    /// Common daily rental rates for tests.
    /// </summary>
    public static class DailyRates
    {
        public static Money Klein => EuroNet(29.00m);
        public static Money Kompakt => EuroNet(39.00m);
        public static Money Mittel => EuroNet(59.00m);
        public static Money Ober => EuroNet(89.00m);
        public static Money Suv => EuroNet(69.00m);
        public static Money Luxus => EuroNet(149.00m);
    }

    /// <summary>
    /// Common extra charges for tests.
    /// </summary>
    public static class Extras
    {
        public static Money Gps => EuroNet(5.00m);
        public static Money ChildSeat => EuroNet(8.00m);
        public static Money AdditionalDriver => EuroNet(10.00m);
        public static Money CrossBorderPermit => EuroNet(15.00m);
    }
}

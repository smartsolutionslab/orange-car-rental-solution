using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Builders;

/// <summary>
/// Helper class for creating common test data.
/// Provides factory methods for frequently used value objects.
/// </summary>
public static class TestDataHelpers
{
    // Common test identifiers
    public static CustomerIdentifier DefaultCustomerId => CustomerIdentifier.New();
    public static VehicleIdentifier DefaultVehicleId => VehicleIdentifier.New();

    // Common test locations
    public static LocationCode BerlinHbf => LocationCode.Of("BER-HBF");
    public static LocationCode MunichAirport => LocationCode.Of("MUC-FLG");
    public static LocationCode FrankfurtAirport => LocationCode.Of("FRA-FLG");
    public static LocationCode HamburgHbf => LocationCode.Of("HAM-HBF");

    // Common test categories
    public static VehicleCategory Compact => VehicleCategory.From("KOMPAKT");
    public static VehicleCategory MidSize => VehicleCategory.From("MITTEL");
    public static VehicleCategory Suv => VehicleCategory.From("SUV");
    public static VehicleCategory UpperClass => VehicleCategory.From("OBER");

    // Helper methods for creating common value objects

    /// <summary>
    /// Creates a booking period starting from today + offset days, lasting the specified duration.
    /// Duration includes both pickup and return dates (e.g., duration=3 means pickup + 2 days).
    /// </summary>
    public static BookingPeriod CreatePeriod(int startDaysFromToday = 7, int durationDays = 3)
    {
        var pickupDate = DateTime.UtcNow.Date.AddDays(startDaysFromToday);
        var returnDate = pickupDate.AddDays(durationDays - 1); // -1 because period includes both dates
        return BookingPeriod.Of(pickupDate, returnDate);
    }

    /// <summary>
    /// Creates a booking period for today (useful for testing active rentals).
    /// Duration includes both pickup and return dates (e.g., duration=3 means pickup + 2 days).
    /// </summary>
    public static BookingPeriod TodayPeriod(int durationDays = 3)
    {
        var pickupDate = DateTime.UtcNow.Date;
        var returnDate = pickupDate.AddDays(durationDays - 1); // -1 because period includes both dates
        return BookingPeriod.Of(pickupDate, returnDate);
    }

    /// <summary>
    /// Creates a Money object in EUR with 19% VAT from a gross amount.
    /// </summary>
    public static Money EuroFromGross(decimal grossAmount)
    {
        return Money.FromGross(grossAmount, 0.19m, Currency.Of("EUR"));
    }

    /// <summary>
    /// Creates a Money object in EUR with 19% VAT from a net amount.
    /// </summary>
    public static Money EuroFromNet(decimal netAmount)
    {
        var grossAmount = netAmount * 1.19m;
        return Money.FromGross(grossAmount, 0.19m, Currency.Of("EUR"));
    }

    /// <summary>
    /// Creates a default EUR currency.
    /// </summary>
    public static Currency Euro => Currency.Of("EUR");
}
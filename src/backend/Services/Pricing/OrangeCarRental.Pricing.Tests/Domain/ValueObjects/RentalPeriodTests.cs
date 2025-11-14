using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Domain.ValueObjects;

public class RentalPeriodTests
{
    [Fact]
    public void Of_WithValidDates_ShouldCreateRentalPeriod()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

        // Act
        var period = RentalPeriod.Of(pickupDate, returnDate);

        // Assert
        period.PickupDate.ShouldBe(pickupDate);
        period.ReturnDate.ShouldBe(returnDate);
        period.TotalDays.ShouldBe(3);
    }

    [Fact]
    public void Of_WithDateTimeDates_ShouldCreateRentalPeriod()
    {
        // Arrange
        var pickupDate = DateTime.UtcNow.AddDays(1);
        var returnDate = DateTime.UtcNow.AddDays(3);

        // Act
        var period = RentalPeriod.Of(pickupDate, returnDate);

        // Assert
        period.PickupDate.ShouldBe(DateOnly.FromDateTime(pickupDate));
        period.ReturnDate.ShouldBe(DateOnly.FromDateTime(returnDate));
    }

    [Fact]
    public void Of_WithOneDayRental_ShouldCalculateTotalDaysCorrectly()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = pickupDate.AddDays(1);

        // Act
        var period = RentalPeriod.Of(pickupDate, returnDate);

        // Assert
        period.TotalDays.ShouldBe(2); // Pickup day + return day
    }

    [Fact]
    public void Of_WithWeekRental_ShouldCalculateTotalDaysCorrectly()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = pickupDate.AddDays(6);

        // Act
        var period = RentalPeriod.Of(pickupDate, returnDate);

        // Assert
        period.TotalDays.ShouldBe(7); // One week
    }

    [Fact]
    public void Of_WithPickupDateInPast_ShouldThrowArgumentException()
    {
        // Arrange
        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => RentalPeriod.Of(pastDate, returnDate));
        ex.Message.ShouldContain("cannot be in the past");
    }

    [Fact]
    public void Of_WithReturnDateBeforePickupDate_ShouldThrowArgumentException()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => RentalPeriod.Of(pickupDate, returnDate));
        ex.Message.ShouldContain("must be after pickup date");
    }

    [Fact]
    public void Of_WithReturnDateEqualToPickupDate_ShouldThrowArgumentException()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => RentalPeriod.Of(date, date));
        ex.Message.ShouldContain("must be after pickup date");
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var pickupDate = new DateOnly(2025, 6, 15);
        var returnDate = new DateOnly(2025, 6, 18);
        var period = RentalPeriod.Of(pickupDate, returnDate);

        // Act
        var result = period.ToString();

        // Assert
        result.ShouldBe("4 day(s) from 2025-06-15 to 2025-06-18");
    }

    [Fact]
    public void Equals_WithSameDates_ShouldBeEqual()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = pickupDate.AddDays(3);
        var period1 = RentalPeriod.Of(pickupDate, returnDate);
        var period2 = RentalPeriod.Of(pickupDate, returnDate);

        // Act & Assert
        period1.ShouldBe(period2);
        (period1 == period2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentDates_ShouldNotBeEqual()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = pickupDate.AddDays(3);
        var period1 = RentalPeriod.Of(pickupDate, returnDate);
        var period2 = RentalPeriod.Of(pickupDate.AddDays(1), returnDate.AddDays(1));

        // Act & Assert
        period1.ShouldNotBe(period2);
        (period1 != period2).ShouldBeTrue();
    }
}

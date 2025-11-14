using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Domain.ValueObjects;

public class BookingPeriodTests
{
    [Fact]
    public void Of_WithValidDates_ShouldCreateBookingPeriod()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

        // Act
        var period = BookingPeriod.Of(pickupDate, returnDate);

        // Assert
        period.PickupDate.ShouldBe(pickupDate);
        period.ReturnDate.ShouldBe(returnDate);
        period.Days.ShouldBe(3);
    }

    [Fact]
    public void Of_WithDateTimeDates_ShouldCreateBookingPeriod()
    {
        // Arrange
        var pickupDate = DateTime.UtcNow.AddDays(1);
        var returnDate = DateTime.UtcNow.AddDays(3);

        // Act
        var period = BookingPeriod.Of(pickupDate, returnDate);

        // Assert
        period.PickupDate.ShouldBe(DateOnly.FromDateTime(pickupDate));
        period.ReturnDate.ShouldBe(DateOnly.FromDateTime(returnDate));
    }

    [Fact]
    public void Of_WithSameDayPickupAndReturn_ShouldCalculateOneDayRental()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Act & Assert
        // Return date must be after pickup date, so same day should throw
        Should.Throw<ArgumentException>(() => BookingPeriod.Of(date, date));
    }

    [Fact]
    public void Of_WithOneDayRental_ShouldCalculateCorrectly()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = pickupDate.AddDays(1);

        // Act
        var period = BookingPeriod.Of(pickupDate, returnDate);

        // Assert
        period.Days.ShouldBe(2); // Pickup day + return day
    }

    [Fact]
    public void Of_WithPickupDateInPast_ShouldThrowArgumentException()
    {
        // Arrange
        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => BookingPeriod.Of(pastDate, returnDate));
        ex.Message.ShouldContain("cannot be in the past");
    }

    [Fact]
    public void Of_WithReturnDateBeforePickupDate_ShouldThrowArgumentException()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var returnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => BookingPeriod.Of(pickupDate, returnDate));
        ex.Message.ShouldContain("must be after pickup date");
    }

    [Fact]
    public void Of_WithReturnDateEqualToPickupDate_ShouldThrowArgumentException()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => BookingPeriod.Of(date, date));
        ex.Message.ShouldContain("must be after pickup date");
    }

    [Fact]
    public void Of_WithRentalPeriodOver90Days_ShouldThrowArgumentException()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = pickupDate.AddDays(91); // 92 days total

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => BookingPeriod.Of(pickupDate, returnDate));
        ex.Message.ShouldContain("cannot exceed 90 days");
    }

    [Fact]
    public void Of_WithExactly90Days_ShouldSucceed()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = pickupDate.AddDays(89); // 90 days total

        // Act
        var period = BookingPeriod.Of(pickupDate, returnDate);

        // Assert
        period.Days.ShouldBe(90);
    }

    [Fact]
    public void OverlapsWith_WithOverlappingPeriods_ShouldReturnTrue()
    {
        // Arrange
        var pickupDate1 = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate1 = pickupDate1.AddDays(5);
        var period1 = BookingPeriod.Of(pickupDate1, returnDate1);

        var pickupDate2 = pickupDate1.AddDays(3); // Overlaps by 2 days
        var returnDate2 = pickupDate2.AddDays(3);
        var period2 = BookingPeriod.Of(pickupDate2, returnDate2);

        // Act
        var overlaps = period1.OverlapsWith(period2);

        // Assert
        overlaps.ShouldBeTrue();
    }

    [Fact]
    public void OverlapsWith_WithNonOverlappingPeriods_ShouldReturnFalse()
    {
        // Arrange
        var pickupDate1 = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate1 = pickupDate1.AddDays(2);
        var period1 = BookingPeriod.Of(pickupDate1, returnDate1);

        var pickupDate2 = returnDate1.AddDays(2); // Starts 1 day after period1 ends
        var returnDate2 = pickupDate2.AddDays(2);
        var period2 = BookingPeriod.Of(pickupDate2, returnDate2);

        // Act
        var overlaps = period1.OverlapsWith(period2);

        // Assert
        overlaps.ShouldBeFalse();
    }

    [Fact]
    public void OverlapsWith_WithConsecutivePeriods_ShouldReturnFalse()
    {
        // Arrange
        var pickupDate1 = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate1 = pickupDate1.AddDays(2);
        var period1 = BookingPeriod.Of(pickupDate1, returnDate1);

        var pickupDate2 = returnDate1.AddDays(1); // Starts the day after period1 ends
        var returnDate2 = pickupDate2.AddDays(2);
        var period2 = BookingPeriod.Of(pickupDate2, returnDate2);

        // Act
        var overlaps = period1.OverlapsWith(period2);

        // Assert
        overlaps.ShouldBeFalse();
    }

    [Fact]
    public void OverlapsWith_WithContainedPeriod_ShouldReturnTrue()
    {
        // Arrange
        var pickupDate1 = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate1 = pickupDate1.AddDays(10);
        var period1 = BookingPeriod.Of(pickupDate1, returnDate1);

        var pickupDate2 = pickupDate1.AddDays(2); // Fully contained
        var returnDate2 = pickupDate2.AddDays(3);
        var period2 = BookingPeriod.Of(pickupDate2, returnDate2);

        // Act
        var overlaps = period1.OverlapsWith(period2);

        // Assert
        overlaps.ShouldBeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var pickupDate = new DateOnly(2025, 6, 15);
        var returnDate = new DateOnly(2025, 6, 18);
        var period = BookingPeriod.Of(pickupDate, returnDate);

        // Act
        var result = period.ToString();

        // Assert
        result.ShouldBe("2025-06-15 to 2025-06-18 (4 days)");
    }

    [Fact]
    public void Equals_WithSameDates_ShouldBeEqual()
    {
        // Arrange
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var returnDate = pickupDate.AddDays(3);
        var period1 = BookingPeriod.Of(pickupDate, returnDate);
        var period2 = BookingPeriod.Of(pickupDate, returnDate);

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
        var period1 = BookingPeriod.Of(pickupDate, returnDate);
        var period2 = BookingPeriod.Of(pickupDate.AddDays(1), returnDate.AddDays(1));

        // Act & Assert
        period1.ShouldNotBe(period2);
        (period1 != period2).ShouldBeTrue();
    }
}

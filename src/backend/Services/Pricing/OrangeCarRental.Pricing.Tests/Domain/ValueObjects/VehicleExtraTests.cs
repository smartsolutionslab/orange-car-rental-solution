using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.VehicleExtra;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Domain.ValueObjects;

public class VehicleExtraTests
{
    [Fact]
    public void GPS_HasCorrectProperties()
    {
        // Act
        var extra = VehicleExtra.GPS;

        // Assert
        extra.Type.ShouldBe(VehicleExtraType.GPS);
        extra.DailyRate.GrossAmount.ShouldBe(5.00m);
        extra.RequiresAdvanceBooking.ShouldBeFalse();
        extra.IsPerRental.ShouldBeFalse();
    }

    [Fact]
    public void ChildSeatInfant_RequiresAdvanceBooking()
    {
        // Act
        var extra = VehicleExtra.ChildSeatInfant;

        // Assert
        extra.Type.ShouldBe(VehicleExtraType.ChildSeatInfant);
        extra.DailyRate.GrossAmount.ShouldBe(8.00m);
        extra.RequiresAdvanceBooking.ShouldBeTrue();
        extra.IsPerRental.ShouldBeFalse();
    }

    [Fact]
    public void WinterTires_IsIncluded()
    {
        // Act
        var extra = VehicleExtra.WinterTires;

        // Assert
        extra.Type.ShouldBe(VehicleExtraType.WinterTires);
        extra.DailyRate.GrossAmount.ShouldBe(0);
        extra.IsPerRental.ShouldBeTrue();
    }

    [Fact]
    public void SnowChains_IsPerRental()
    {
        // Act
        var extra = VehicleExtra.SnowChains;

        // Assert
        extra.Type.ShouldBe(VehicleExtraType.SnowChains);
        extra.DailyRate.GrossAmount.ShouldBe(15.00m);
        extra.IsPerRental.ShouldBeTrue();
    }

    [Fact]
    public void CalculateCost_PerDay_MultipliesByDays()
    {
        // Arrange
        var extra = VehicleExtra.GPS; // 5€/day

        // Act
        var cost = extra.CalculateCost(7);

        // Assert - 7 days * 5€ = 35€
        cost.GrossAmount.ShouldBe(35.00m);
    }

    [Fact]
    public void CalculateCost_PerRental_ReturnsFlatFee()
    {
        // Arrange
        var extra = VehicleExtra.SnowChains; // 15€ per rental

        // Act
        var cost = extra.CalculateCost(7);

        // Assert - Per rental, so still 15€
        cost.GrossAmount.ShouldBe(15.00m);
    }

    [Fact]
    public void GetGermanDisplayName_ReturnsCorrectName()
    {
        // Assert
        VehicleExtra.GPS.GetGermanDisplayName().ShouldBe("Navigationssystem");
        VehicleExtra.ChildSeatInfant.GetGermanDisplayName().ShouldBe("Babyschale (0-12 Monate)");
        VehicleExtra.ChildSeatToddler.GetGermanDisplayName().ShouldBe("Kindersitz (1-4 Jahre)");
        VehicleExtra.WinterTires.GetGermanDisplayName().ShouldBe("Winterreifen");
        VehicleExtra.SnowChains.GetGermanDisplayName().ShouldBe("Schneeketten");
        VehicleExtra.RoofRack.GetGermanDisplayName().ShouldBe("Dachgepäckträger");
        VehicleExtra.AdditionalDriver.GetGermanDisplayName().ShouldBe("Zusatzfahrer");
    }

    [Fact]
    public void GetEnglishDisplayName_ReturnsCorrectName()
    {
        // Assert
        VehicleExtra.GPS.GetEnglishDisplayName().ShouldBe("GPS Navigation");
        VehicleExtra.ChildSeatInfant.GetEnglishDisplayName().ShouldBe("Infant Car Seat (0-12 months)");
        VehicleExtra.WinterTires.GetEnglishDisplayName().ShouldBe("Winter Tires");
        VehicleExtra.CrossBorderPermit.GetEnglishDisplayName().ShouldBe("Cross-Border Travel Permit");
    }

    [Fact]
    public void GetAllExtras_Returns12Extras()
    {
        // Act
        var extras = VehicleExtra.GetAllExtras();

        // Assert
        extras.Count.ShouldBe(12);
    }

    [Fact]
    public void FromType_ReturnsCorrectExtra()
    {
        // Act & Assert
        VehicleExtra.FromType(VehicleExtraType.GPS).ShouldBe(VehicleExtra.GPS);
        VehicleExtra.FromType(VehicleExtraType.ChildSeatInfant).ShouldBe(VehicleExtra.ChildSeatInfant);
        VehicleExtra.FromType(VehicleExtraType.WinterTires).ShouldBe(VehicleExtra.WinterTires);
        VehicleExtra.FromType(VehicleExtraType.AdditionalDriver).ShouldBe(VehicleExtra.AdditionalDriver);
    }

    [Fact]
    public void FromType_InvalidType_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => VehicleExtra.FromType((VehicleExtraType)999));
    }

    [Theory]
    [InlineData(VehicleExtraType.GPS, false)]
    [InlineData(VehicleExtraType.ChildSeatInfant, true)]
    [InlineData(VehicleExtraType.ChildSeatToddler, true)]
    [InlineData(VehicleExtraType.BoosterSeat, true)]
    [InlineData(VehicleExtraType.WinterTires, false)]
    [InlineData(VehicleExtraType.SnowChains, true)]
    [InlineData(VehicleExtraType.RoofRack, true)]
    [InlineData(VehicleExtraType.AdditionalDriver, false)]
    public void RequiresAdvanceBooking_ReturnsCorrectValue(VehicleExtraType type, bool expected)
    {
        // Act
        var extra = VehicleExtra.FromType(type);

        // Assert
        extra.RequiresAdvanceBooking.ShouldBe(expected);
    }

    [Theory]
    [InlineData(VehicleExtraType.GPS, false)]
    [InlineData(VehicleExtraType.WinterTires, true)]
    [InlineData(VehicleExtraType.SnowChains, true)]
    [InlineData(VehicleExtraType.CrossBorderPermit, true)]
    [InlineData(VehicleExtraType.AdditionalDriver, false)]
    public void IsPerRental_ReturnsCorrectValue(VehicleExtraType type, bool expected)
    {
        // Act
        var extra = VehicleExtra.FromType(type);

        // Assert
        extra.IsPerRental.ShouldBe(expected);
    }
}

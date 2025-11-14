using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Domain.ValueObjects;

public class LocationTests
{
    [Fact]
    public void BerlinHauptbahnhof_ShouldHaveCorrectDetails()
    {
        // Assert
        Location.BerlinHauptbahnhof.Code.Value.ShouldBe("BER-HBF");
        Location.BerlinHauptbahnhof.Name.Value.ShouldBe("Berlin Hauptbahnhof");
        Location.BerlinHauptbahnhof.Address.City.Value.ShouldBe("Berlin");
        Location.BerlinHauptbahnhof.Address.PostalCode.Value.ShouldBe("10557");
    }

    [Fact]
    public void MunichFlughafen_ShouldHaveCorrectDetails()
    {
        // Assert
        Location.MunichFlughafen.Code.Value.ShouldBe("MUC-FLG");
        Location.MunichFlughafen.Name.Value.ShouldBe("München Flughafen");
        Location.MunichFlughafen.Address.City.Value.ShouldBe("München");
        Location.MunichFlughafen.Address.PostalCode.Value.ShouldBe("85356");
    }

    [Fact]
    public void FrankfurtFlughafen_ShouldHaveCorrectDetails()
    {
        // Assert
        Location.FrankfurtFlughafen.Code.Value.ShouldBe("FRA-FLG");
        Location.FrankfurtFlughafen.Name.Value.ShouldBe("Frankfurt Flughafen");
        Location.FrankfurtFlughafen.Address.City.Value.ShouldBe("Frankfurt");
    }

    [Fact]
    public void HamburgHauptbahnhof_ShouldHaveCorrectDetails()
    {
        // Assert
        Location.HamburgHauptbahnhof.Code.Value.ShouldBe("HAM-HBF");
        Location.HamburgHauptbahnhof.Name.Value.ShouldBe("Hamburg Hauptbahnhof");
        Location.HamburgHauptbahnhof.Address.City.Value.ShouldBe("Hamburg");
    }

    [Fact]
    public void KolnHauptbahnhof_ShouldHaveCorrectDetails()
    {
        // Assert
        Location.KolnHauptbahnhof.Code.Value.ShouldBe("CGN-HBF");
        Location.KolnHauptbahnhof.Name.Value.ShouldBe("Köln Hauptbahnhof");
        Location.KolnHauptbahnhof.Address.City.Value.ShouldBe("Köln");
    }

    [Fact]
    public void All_ShouldReturnAllLocations()
    {
        // Act
        var allLocations = Location.All;

        // Assert
        allLocations.Count.ShouldBe(5);
        allLocations.ShouldContain(Location.BerlinHauptbahnhof);
        allLocations.ShouldContain(Location.MunichFlughafen);
        allLocations.ShouldContain(Location.FrankfurtFlughafen);
        allLocations.ShouldContain(Location.HamburgHauptbahnhof);
        allLocations.ShouldContain(Location.KolnHauptbahnhof);
    }

    [Fact]
    public void FromCode_WithValidCode_ShouldReturnLocation()
    {
        // Act
        var location = Location.FromCode(LocationCode.Of("BER-HBF"));

        // Assert
        location.ShouldBe(Location.BerlinHauptbahnhof);
    }

    [Fact]
    public void FromCode_WithInvalidCode_ShouldThrowArgumentException()
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() =>
            Location.FromCode(LocationCode.Of("INVALID")));
        ex.Message.ShouldContain("Unknown location code");
    }

    [Fact]
    public void Of_WithCustomLocation_ShouldCreateLocation()
    {
        // Act
        var location = Location.Of(
            LocationCode.Of("TEST-LOC"),
            LocationName.Of("Test Location"),
            Address.Of("Test Street", "Test City", "12345"));

        // Assert
        location.Code.Value.ShouldBe("TEST-LOC");
        location.Name.Value.ShouldBe("Test Location");
        location.Address.City.Value.ShouldBe("Test City");
    }

    [Fact]
    public void Of_BackwardCompatibility_ShouldCreateLocation()
    {
        // Act
        var location = Location.Of("CUSTOM", "Stuttgart", "Hauptstraße 1", "70173");

        // Assert
        location.Code.Value.ShouldBe("CUSTOM");
        location.Name.Value.ShouldBe("Stuttgart");
        location.Address.City.Value.ShouldBe("Stuttgart");
    }

    [Fact]
    public void ToString_ShouldReturnNameAndCode()
    {
        // Act
        var result = Location.BerlinHauptbahnhof.ToString();

        // Assert
        result.ShouldBe("Berlin Hauptbahnhof (BER-HBF)");
    }

    [Fact]
    public void Equals_WithSameLocation_ShouldBeEqual()
    {
        // Arrange
        var location1 = Location.FromCode(LocationCode.Of("BER-HBF"));
        var location2 = Location.BerlinHauptbahnhof;

        // Act & Assert
        location1.ShouldBe(location2);
        (location1 == location2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentLocations_ShouldNotBeEqual()
    {
        // Arrange
        var location1 = Location.BerlinHauptbahnhof;
        var location2 = Location.MunichFlughafen;

        // Act & Assert
        location1.ShouldNotBe(location2);
        (location1 != location2).ShouldBeTrue();
    }
}

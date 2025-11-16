using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Domain.ValueObjects;

public class VehicleNameTests
{
    [Theory]
    [InlineData("BMW X5")]
    [InlineData("VW Golf")]
    [InlineData("Mercedes-Benz E-Klasse")]
    [InlineData("Audi A4 Avant")]
    public void Of_WithValidName_ShouldCreateVehicleName(string validName)
    {
        // Act
        var vehicleName = VehicleName.Of(validName);

        // Assert
        vehicleName.Value.ShouldBe(validName);
    }

    [Fact]
    public void Of_WithLeadingAndTrailingWhitespace_ShouldTrim()
    {
        // Act
        var vehicleName = VehicleName.Of("  BMW X5  ");

        // Assert
        vehicleName.Value.ShouldBe("BMW X5");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Of_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidName)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => VehicleName.Of(invalidName));
    }

    [Fact]
    public void Of_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => VehicleName.Of(null!));
    }

    [Fact]
    public void Of_WithTooLongName_ShouldThrowArgumentException()
    {
        // Arrange - 101 characters (exceeds max of 100)
        var longName = new string('A', 101);

        // Act & Assert
        Should.Throw<ArgumentException>(() => VehicleName.Of(longName));
    }

    [Fact]
    public void Of_WithExactly100Characters_ShouldSucceed()
    {
        // Arrange - Exactly 100 characters (edge case)
        var name = new string('A', 100);

        // Act
        var vehicleName = VehicleName.Of(name);

        // Assert
        vehicleName.Value.Length.ShouldBe(100);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertToString()
    {
        // Arrange
        var vehicleName = VehicleName.Of("BMW X5");

        // Act
        string nameString = vehicleName;

        // Assert
        nameString.ShouldBe("BMW X5");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var vehicleName = VehicleName.Of("BMW X5");

        // Act
        var result = vehicleName.ToString();

        // Assert
        result.ShouldBe("BMW X5");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var name1 = VehicleName.Of("BMW X5");
        var name2 = VehicleName.Of("BMW X5");

        // Act & Assert
        name1.ShouldBe(name2);
        (name1 == name2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var name1 = VehicleName.Of("BMW X5");
        var name2 = VehicleName.Of("Audi Q7");

        // Act & Assert
        name1.ShouldNotBe(name2);
        (name1 != name2).ShouldBeTrue();
    }
}

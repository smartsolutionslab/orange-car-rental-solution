using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Domain.ValueObjects;

public class ManufacturerTests
{
    [Theory]
    [InlineData("BMW")]
    [InlineData("Volkswagen")]
    [InlineData("Mercedes-Benz")]
    [InlineData("Audi")]
    [InlineData("Porsche")]
    [InlineData("Opel")]
    public void Of_WithValidManufacturer_ShouldCreateManufacturer(string validManufacturer)
    {
        // Act
        var manufacturer = Manufacturer.Of(validManufacturer);

        // Assert
        manufacturer.Value.ShouldBe(validManufacturer);
    }

    [Fact]
    public void Of_WithLeadingAndTrailingWhitespace_ShouldTrim()
    {
        // Act
        var manufacturer = Manufacturer.Of("  BMW  ");

        // Assert
        manufacturer.Value.ShouldBe("BMW");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Of_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidManufacturer)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => Manufacturer.Of(invalidManufacturer));
    }

    [Fact]
    public void Of_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => Manufacturer.Of(null!));
    }

    [Fact]
    public void Of_WithTooLongName_ShouldThrowArgumentException()
    {
        // Arrange - 101 characters (exceeds max of 100)
        var longName = new string('A', 101);

        // Act & Assert
        Should.Throw<ArgumentException>(() => Manufacturer.Of(longName));
    }

    [Fact]
    public void Of_WithExactly100Characters_ShouldSucceed()
    {
        // Arrange - Exactly 100 characters (edge case)
        var name = new string('A', 100);

        // Act
        var manufacturer = Manufacturer.Of(name);

        // Assert
        manufacturer.Value.Length.ShouldBe(100);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertToString()
    {
        // Arrange
        var manufacturer = Manufacturer.Of("BMW");

        // Act
        string manufacturerString = manufacturer;

        // Assert
        manufacturerString.ShouldBe("BMW");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var manufacturer = Manufacturer.Of("BMW");

        // Act
        var result = manufacturer.ToString();

        // Assert
        result.ShouldBe("BMW");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var manufacturer1 = Manufacturer.Of("BMW");
        var manufacturer2 = Manufacturer.Of("BMW");

        // Act & Assert
        manufacturer1.ShouldBe(manufacturer2);
        (manufacturer1 == manufacturer2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var manufacturer1 = Manufacturer.Of("BMW");
        var manufacturer2 = Manufacturer.Of("Audi");

        // Act & Assert
        manufacturer1.ShouldNotBe(manufacturer2);
        (manufacturer1 != manufacturer2).ShouldBeTrue();
    }
}

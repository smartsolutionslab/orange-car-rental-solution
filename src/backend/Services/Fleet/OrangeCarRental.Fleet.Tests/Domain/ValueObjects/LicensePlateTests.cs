using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Domain.ValueObjects;

public class LicensePlateTests
{
    [Theory]
    [InlineData("B AB 1234")]
    [InlineData("M XY 999")]
    [InlineData("HH AA 1")]
    [InlineData("B-AB-1234")]
    [InlineData("BAB1234")]
    [InlineData("M AB 1234E")]
    [InlineData("M AB 1234H")]
    public void From_WithValidGermanLicensePlate_ShouldCreate(string validPlate)
    {
        // Act
        var licensePlate = LicensePlate.From(validPlate);

        // Assert
        licensePlate.Value.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void From_WithLowercaseInput_ShouldNormalizeToUppercase()
    {
        // Act
        var licensePlate = LicensePlate.From("b ab 1234");

        // Assert
        licensePlate.Value.ShouldBe("B AB 1234");
    }

    [Fact]
    public void From_WithLeadingAndTrailingWhitespace_ShouldTrim()
    {
        // Act
        var licensePlate = LicensePlate.From("  B AB 1234  ");

        // Assert
        licensePlate.Value.ShouldBe("B AB 1234");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidPlate)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => LicensePlate.From(invalidPlate));
    }

    [Fact]
    public void From_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => LicensePlate.From(null!));
    }

    [Theory]
    [InlineData("12345")]           // Only digits - missing letters
    [InlineData("ABCDEFGH")]        // Only letters - missing digits
    [InlineData("B AB 12345")]      // 5 digits - max is 4
    [InlineData("ABCD AB 1234")]    // 4 letter city code - max is 3
    [InlineData("B ABC 1234")]      // 3 letter second group - max is 2
    public void From_WithInvalidFormat_ShouldThrowArgumentException(string invalidPlate)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => LicensePlate.From(invalidPlate));
    }

    [Fact]
    public void From_WithTooLongPlate_ShouldThrowArgumentException()
    {
        // Arrange - 21 characters (exceeds max of 20)
        var longPlate = new string('A', 21);

        // Act & Assert
        Should.Throw<ArgumentException>(() => LicensePlate.From(longPlate));
    }

    [Theory]
    [InlineData("B AB 1234", true)]
    [InlineData("M XY 999", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("INVALID", false)]
    [InlineData("12345", false)]
    public void TryParse_ShouldReturnExpectedResult(string? input, bool expectedResult)
    {
        // Act
        var result = LicensePlate.TryParse(input, out var licensePlate);

        // Assert
        result.ShouldBe(expectedResult);
        if (expectedResult)
        {
            licensePlate.Value.ShouldNotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void TryParse_WithNull_ShouldReturnFalse()
    {
        // Act
        var result = LicensePlate.TryParse(null, out _);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertToString()
    {
        // Arrange
        var licensePlate = LicensePlate.From("B AB 1234");

        // Act
        string plateString = licensePlate;

        // Assert
        plateString.ShouldBe("B AB 1234");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var licensePlate = LicensePlate.From("B AB 1234");

        // Act
        var result = licensePlate.ToString();

        // Assert
        result.ShouldBe("B AB 1234");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var plate1 = LicensePlate.From("B AB 1234");
        var plate2 = LicensePlate.From("B AB 1234");

        // Act & Assert
        plate1.ShouldBe(plate2);
        (plate1 == plate2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var plate1 = LicensePlate.From("B AB 1234");
        var plate2 = LicensePlate.From("M XY 999");

        // Act & Assert
        plate1.ShouldNotBe(plate2);
        (plate1 != plate2).ShouldBeTrue();
    }
}

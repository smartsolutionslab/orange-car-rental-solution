using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Domain.ValueObjects;

public class LocationCodeTests
{
    [Theory]
    [InlineData("BER-HBF")]
    [InlineData("MUC-FLG")]
    [InlineData("FRA-FLG")]
    [InlineData("HAM-HBF")]
    [InlineData("CGN-HBF")]
    public void Of_WithValidCode_ShouldCreateLocationCode(string validCode)
    {
        // Act
        var locationCode = LocationCode.Of(validCode);

        // Assert
        locationCode.Value.ShouldBe(validCode);
    }

    [Fact]
    public void Of_ShouldConvertToUpperCase()
    {
        // Act
        var locationCode = LocationCode.Of("ber-hbf");

        // Assert
        locationCode.Value.ShouldBe("BER-HBF");
    }

    [Fact]
    public void Of_ShouldTrimWhitespace()
    {
        // Act
        var locationCode = LocationCode.Of("  BER-HBF  ");

        // Assert
        locationCode.Value.ShouldBe("BER-HBF");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Of_WithNullOrWhitespace_ShouldThrowArgumentException(string invalidCode)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => LocationCode.Of(invalidCode));
    }

    [Fact]
    public void Of_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => LocationCode.Of(null!));
    }

    [Fact]
    public void Of_WithCodeTooShort_ShouldThrowArgumentException()
    {
        // Arrange - 2 characters (less than minimum 3)
        var shortCode = "AB";

        // Act & Assert
        Should.Throw<ArgumentException>(() => LocationCode.Of(shortCode));
    }

    [Fact]
    public void Of_WithCodeTooLong_ShouldThrowArgumentException()
    {
        // Arrange - 21 characters (exceeds max of 20)
        var longCode = new string('A', 21);

        // Act & Assert
        Should.Throw<ArgumentException>(() => LocationCode.Of(longCode));
    }

    [Fact]
    public void Of_WithExactly3Characters_ShouldSucceed()
    {
        // Arrange - Minimum length (edge case)
        var code = "BER";

        // Act
        var locationCode = LocationCode.Of(code);

        // Assert
        locationCode.Value.ShouldBe("BER");
    }

    [Fact]
    public void Of_WithExactly20Characters_ShouldSucceed()
    {
        // Arrange - Maximum length (edge case)
        var code = new string('A', 20);

        // Act
        var locationCode = LocationCode.Of(code);

        // Assert
        locationCode.Value.Length.ShouldBe(20);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertToString()
    {
        // Arrange
        var locationCode = LocationCode.Of("BER-HBF");

        // Act
        string codeString = locationCode;

        // Assert
        codeString.ShouldBe("BER-HBF");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var locationCode = LocationCode.Of("BER-HBF");

        // Act
        var result = locationCode.ToString();

        // Assert
        result.ShouldBe("BER-HBF");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var code1 = LocationCode.Of("BER-HBF");
        var code2 = LocationCode.Of("ber-hbf"); // Should be normalized to uppercase

        // Act & Assert
        code1.ShouldBe(code2);
        (code1 == code2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var code1 = LocationCode.Of("BER-HBF");
        var code2 = LocationCode.Of("MUC-FLG");

        // Act & Assert
        code1.ShouldNotBe(code2);
        (code1 != code2).ShouldBeTrue();
    }
}

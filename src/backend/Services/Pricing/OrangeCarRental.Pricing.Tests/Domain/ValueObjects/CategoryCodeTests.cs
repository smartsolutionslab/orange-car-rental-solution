using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Domain.ValueObjects;

public class CategoryCodeTests
{
    [Theory]
    [InlineData("KLEIN")]
    [InlineData("KOMPAKT")]
    [InlineData("MITTEL")]
    [InlineData("OBER")]
    [InlineData("SUV")]
    [InlineData("KOMBI")]
    [InlineData("TRANS")]
    [InlineData("LUXUS")]
    public void Of_WithValidCode_ShouldCreateCategoryCode(string validCode)
    {
        // Act
        var categoryCode = CategoryCode.Of(validCode);

        // Assert
        categoryCode.Value.ShouldBe(validCode);
    }

    [Fact]
    public void Of_ShouldConvertToUpperCase()
    {
        // Act
        var categoryCode = CategoryCode.Of("klein");

        // Assert
        categoryCode.Value.ShouldBe("KLEIN");
    }

    [Fact]
    public void Of_ShouldTrimWhitespace()
    {
        // Act
        var categoryCode = CategoryCode.Of("  KLEIN  ");

        // Assert
        categoryCode.Value.ShouldBe("KLEIN");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Of_WithNullOrWhitespace_ShouldThrowArgumentException(string invalidCode)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => CategoryCode.Of(invalidCode));
    }

    [Fact]
    public void Of_WithCodeTooLong_ShouldThrowArgumentException()
    {
        // Arrange - 21 characters (exceeds max of 20)
        var longCode = new string('A', 21);

        // Act & Assert
        Should.Throw<ArgumentException>(() => CategoryCode.Of(longCode));
    }

    [Fact]
    public void Of_WithExactly20Characters_ShouldSucceed()
    {
        // Arrange - Maximum length (edge case)
        var code = new string('A', 20);

        // Act
        var categoryCode = CategoryCode.Of(code);

        // Assert
        categoryCode.Value.Length.ShouldBe(20);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertToString()
    {
        // Arrange
        var categoryCode = CategoryCode.Of("KLEIN");

        // Act
        string codeString = categoryCode;

        // Assert
        codeString.ShouldBe("KLEIN");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var categoryCode = CategoryCode.Of("KLEIN");

        // Act
        var result = categoryCode.ToString();

        // Assert
        result.ShouldBe("KLEIN");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var code1 = CategoryCode.Of("KLEIN");
        var code2 = CategoryCode.Of("klein"); // Should be normalized to uppercase

        // Act & Assert
        code1.ShouldBe(code2);
        (code1 == code2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var code1 = CategoryCode.Of("KLEIN");
        var code2 = CategoryCode.Of("MITTEL");

        // Act & Assert
        code1.ShouldNotBe(code2);
        (code1 != code2).ShouldBeTrue();
    }
}

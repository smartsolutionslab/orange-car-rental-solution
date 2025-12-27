using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Domain.ValueObjects;

public class VATIdTests
{
    [Fact]
    public void Create_WithValidGermanVATId_Succeeds()
    {
        // Act
        var vatId = VATId.Create("DE123456789");

        // Assert
        vatId.Value.ShouldBe("DE123456789");
        vatId.CountryCode.ShouldBe("DE");
        vatId.NumericPart.ShouldBe("123456789");
        vatId.IsGerman.ShouldBeTrue();
    }

    [Fact]
    public void Create_WithLowerCase_NormalizesToUpperCase()
    {
        // Act
        var vatId = VATId.Create("de123456789");

        // Assert
        vatId.Value.ShouldBe("DE123456789");
    }

    [Fact]
    public void Create_WithSpaces_RemovesSpaces()
    {
        // Act
        var vatId = VATId.Create("DE 123 456 789");

        // Assert
        vatId.Value.ShouldBe("DE123456789");
    }

    [Fact]
    public void Create_WithEmptyString_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => VATId.Create(""));
    }

    [Fact]
    public void Create_WithWhitespace_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => VATId.Create("   "));
    }

    [Fact]
    public void Create_WithInvalidFormat_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => VATId.Create("DE12345678")); // Too few digits
        Should.Throw<ArgumentException>(() => VATId.Create("DE1234567890")); // Too many digits
        Should.Throw<ArgumentException>(() => VATId.Create("FR123456789")); // Wrong country code
        Should.Throw<ArgumentException>(() => VATId.Create("123456789")); // No country code
        Should.Throw<ArgumentException>(() => VATId.Create("DEABCDEFGHI")); // Letters instead of digits
    }

    [Fact]
    public void Formatted_ReturnsFormattedVATId()
    {
        // Arrange
        var vatId = VATId.Create("DE123456789");

        // Act
        var formatted = vatId.Formatted;

        // Assert
        formatted.ShouldBe("DE 123456789");
    }

    [Fact]
    public void TryCreate_WithValidVATId_ReturnsTrue()
    {
        // Act
        var result = VATId.TryCreate("DE123456789", out var vatId);

        // Assert
        result.ShouldBeTrue();
        vatId.Value.ShouldBe("DE123456789");
    }

    [Fact]
    public void TryCreate_WithInvalidVATId_ReturnsFalse()
    {
        // Act
        var result = VATId.TryCreate("invalid", out var vatId);

        // Assert
        result.ShouldBeFalse();
        vatId.ShouldBe(default);
    }

    [Fact]
    public void ValidateCheckDigit_WithValidFormat_ReturnsTrue()
    {
        // Arrange
        var vatId = VATId.Create("DE123456789");

        // Act
        var isValid = vatId.ValidateCheckDigit();

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        var vatId = VATId.Create("DE123456789");

        // Act
        var result = vatId.ToString();

        // Assert
        result.ShouldBe("DE123456789");
    }

    [Fact]
    public void Equality_SameValue_AreEqual()
    {
        // Arrange
        var vatId1 = VATId.Create("DE123456789");
        var vatId2 = VATId.Create("de 123 456 789"); // Different casing and spacing

        // Act & Assert
        vatId1.ShouldBe(vatId2);
    }
}

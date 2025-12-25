using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Tests.Sepa;

public class BICTests
{
    // Valid German BIC (Deutsche Bank)
    private const string ValidGermanBIC = "DEUTDEDBFRA";

    [Fact]
    public void Create_ValidBIC11_CreatesBIC()
    {
        // Act
        var bic = BIC.Create(ValidGermanBIC);

        // Assert
        bic.Value.ShouldBe(ValidGermanBIC);
        bic.BankCode.ShouldBe("DEUT");
        bic.CountryCode.ShouldBe("DE");
        bic.LocationCode.ShouldBe("DB");
        bic.BranchCode.ShouldBe("FRA");
        bic.IsGerman.ShouldBeTrue();
    }

    [Fact]
    public void Create_ValidBIC8_CreatesBIC()
    {
        // Arrange
        var bic8 = "DEUTDEFF";

        // Act
        var bic = BIC.Create(bic8);

        // Assert
        bic.Value.ShouldBe(bic8);
        bic.BranchCode.ShouldBe("XXX"); // Default branch code for 8-char BIC
    }

    [Fact]
    public void Create_ValidBICWithSpaces_NormalizesBIC()
    {
        // Arrange
        var bicWithSpaces = "DEUT DE DB FRA";

        // Act
        var bic = BIC.Create(bicWithSpaces);

        // Assert
        bic.Value.ShouldBe(ValidGermanBIC);
    }

    [Fact]
    public void Create_ValidBICLowercase_NormalizesToUppercase()
    {
        // Arrange
        var lowercaseBic = "deutdedbfra";

        // Act
        var bic = BIC.Create(lowercaseBic);

        // Assert
        bic.Value.ShouldBe(ValidGermanBIC);
    }

    [Fact]
    public void Create_InvalidLength_ThrowsArgumentException()
    {
        // Arrange
        var invalidBic = "DEUT"; // Too short

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => BIC.Create(invalidBic));
        ex.Message.ShouldContain("8 or 11 characters");
    }

    [Fact]
    public void Create_InvalidFormat_ThrowsArgumentException()
    {
        // Arrange - Numbers in bank code (should be letters only)
        var invalidBic = "1234DEFF";

        // Act & Assert
        Should.Throw<ArgumentException>(() => BIC.Create(invalidBic));
    }

    [Fact]
    public void Create_NullOrWhitespace_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => BIC.Create(null!));
        Should.Throw<ArgumentException>(() => BIC.Create(""));
        Should.Throw<ArgumentException>(() => BIC.Create("   "));
    }

    [Fact]
    public void Create_ValidFrenchBIC_CreatesBIC()
    {
        // Arrange
        var frenchBic = "BNPAFRPP";

        // Act
        var bic = BIC.Create(frenchBic);

        // Assert
        bic.CountryCode.ShouldBe("FR");
        bic.IsGerman.ShouldBeFalse();
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Act
        var bic = BIC.Create(ValidGermanBIC);

        // Assert
        bic.ToString().ShouldBe(ValidGermanBIC);
    }

    [Fact]
    public void TryCreate_ValidBIC_ReturnsTrue()
    {
        // Act
        var result = BIC.TryCreate(ValidGermanBIC, out var bic);

        // Assert
        result.ShouldBeTrue();
        bic.ShouldNotBeNull();
        bic!.Value.ShouldBe(ValidGermanBIC);
    }

    [Fact]
    public void TryCreate_InvalidBIC_ReturnsFalse()
    {
        // Act
        var result = BIC.TryCreate("invalid", out var bic);

        // Assert
        result.ShouldBeFalse();
        bic.ShouldBeNull();
    }

    [Fact]
    public void TryCreate_NullBIC_ReturnsFalse()
    {
        // Act
        var result = BIC.TryCreate(null, out var bic);

        // Assert
        result.ShouldBeFalse();
        bic.ShouldBeNull();
    }

    [Theory]
    [InlineData("DEUTDEFF")]      // Deutsche Bank (8-char)
    [InlineData("DEUTDEDBFRA")]   // Deutsche Bank Frankfurt (11-char)
    [InlineData("COBADEFFXXX")]   // Commerzbank
    [InlineData("INGDDEFF")]      // ING-DiBa
    public void Create_ValidGermanBICs_Succeeds(string bicValue)
    {
        // Act
        var bic = BIC.Create(bicValue);

        // Assert
        bic.IsGerman.ShouldBeTrue();
    }
}

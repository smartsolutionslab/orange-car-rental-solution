using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Tests.Sepa;

public class IBANTests
{
    // Valid German IBAN (Deutsche Bank test IBAN)
    private const string ValidGermanIBAN = "DE89370400440532013000";

    [Fact]
    public void Create_ValidGermanIBAN_CreatesIBAN()
    {
        // Act
        var iban = IBAN.Create(ValidGermanIBAN);

        // Assert
        iban.Value.ShouldBe(ValidGermanIBAN);
        iban.CountryCode.ShouldBe("DE");
        iban.IsGerman.ShouldBeTrue();
    }

    [Fact]
    public void Create_ValidIBANWithSpaces_NormalizesIBAN()
    {
        // Arrange
        var ibanWithSpaces = "DE89 3704 0044 0532 0130 00";

        // Act
        var iban = IBAN.Create(ibanWithSpaces);

        // Assert
        iban.Value.ShouldBe(ValidGermanIBAN);
    }

    [Fact]
    public void Create_ValidIBANLowercase_NormalizesToUppercase()
    {
        // Arrange
        var lowercaseIban = "de89370400440532013000";

        // Act
        var iban = IBAN.Create(lowercaseIban);

        // Assert
        iban.Value.ShouldBe(ValidGermanIBAN);
    }

    [Fact]
    public void Create_GermanIBANWrongLength_ThrowsArgumentException()
    {
        // Arrange - German IBAN must be 22 characters
        var shortIban = "DE8937040044053201300";

        // Act & Assert
        Should.Throw<ArgumentException>(() => IBAN.Create(shortIban));
    }

    [Fact]
    public void Create_InvalidCheckDigits_ThrowsArgumentException()
    {
        // Arrange - Valid format but wrong check digits
        var invalidIban = "DE00370400440532013000";

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => IBAN.Create(invalidIban));
        ex.Message.ShouldContain("check digits");
    }

    [Fact]
    public void Create_InvalidFormat_ThrowsArgumentException()
    {
        // Arrange
        var invalidIban = "123456";

        // Act & Assert
        Should.Throw<ArgumentException>(() => IBAN.Create(invalidIban));
    }

    [Fact]
    public void Create_NullOrWhitespace_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => IBAN.Create(null!));
        Should.Throw<ArgumentException>(() => IBAN.Create(""));
        Should.Throw<ArgumentException>(() => IBAN.Create("   "));
    }

    [Fact]
    public void Create_ValidFrenchIBAN_CreatesIBAN()
    {
        // Arrange - Valid French IBAN
        var frenchIban = "FR7630006000011234567890189";

        // Act
        var iban = IBAN.Create(frenchIban);

        // Assert
        iban.CountryCode.ShouldBe("FR");
        iban.IsGerman.ShouldBeFalse();
    }

    [Fact]
    public void CheckDigits_ReturnsCorrectDigits()
    {
        // Act
        var iban = IBAN.Create(ValidGermanIBAN);

        // Assert
        iban.CheckDigits.ShouldBe("89");
    }

    [Fact]
    public void BBAN_ReturnsCorrectBBAN()
    {
        // Act
        var iban = IBAN.Create(ValidGermanIBAN);

        // Assert
        iban.BBAN.ShouldBe("370400440532013000");
    }

    [Fact]
    public void Formatted_ReturnsSpacedIBAN()
    {
        // Act
        var iban = IBAN.Create(ValidGermanIBAN);

        // Assert
        iban.Formatted.ShouldBe("DE89 3704 0044 0532 0130 00");
    }

    [Fact]
    public void ToString_ReturnsFormattedIBAN()
    {
        // Act
        var iban = IBAN.Create(ValidGermanIBAN);

        // Assert
        iban.ToString().ShouldBe("DE89 3704 0044 0532 0130 00");
    }

    [Fact]
    public void TryCreate_ValidIBAN_ReturnsTrue()
    {
        // Act
        var result = IBAN.TryCreate(ValidGermanIBAN, out var iban);

        // Assert
        result.ShouldBeTrue();
        iban.ShouldNotBeNull();
        iban!.Value.ShouldBe(ValidGermanIBAN);
    }

    [Fact]
    public void TryCreate_InvalidIBAN_ReturnsFalse()
    {
        // Act
        var result = IBAN.TryCreate("invalid", out var iban);

        // Assert
        result.ShouldBeFalse();
        iban.ShouldBeNull();
    }

    [Fact]
    public void TryCreate_NullIBAN_ReturnsFalse()
    {
        // Act
        var result = IBAN.TryCreate(null, out var iban);

        // Assert
        result.ShouldBeFalse();
        iban.ShouldBeNull();
    }

    [Fact]
    public void Parse_ValidIBAN_ReturnsIBAN()
    {
        // Act
        var iban = IBAN.Parse(ValidGermanIBAN);

        // Assert
        iban.Value.ShouldBe(ValidGermanIBAN);
    }

    [Theory]
    [InlineData("DE89370400440532013000")] // Deutsche Bank
    [InlineData("DE75512108001245126199")] // ING-DiBa
    [InlineData("DE27100777770209299700")] // noris bank
    public void Create_ValidGermanTestIBANs_Succeeds(string iban)
    {
        // Act
        var result = IBAN.Create(iban);

        // Assert
        result.IsGerman.ShouldBeTrue();
    }
}

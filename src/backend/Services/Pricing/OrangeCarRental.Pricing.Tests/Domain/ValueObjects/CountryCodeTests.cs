using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.CrossBorder;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Domain.ValueObjects;

public class CountryCodeTests
{
    [Fact]
    public void Create_ValidCode_ReturnsCountryCode()
    {
        // Act
        var code = CountryCode.Create("de");

        // Assert
        code.Value.ShouldBe("DE");
    }

    [Fact]
    public void Create_WithSpaces_NormalizesCode()
    {
        // Act
        var code = CountryCode.Create(" at ");

        // Assert
        code.Value.ShouldBe("AT");
    }

    [Fact]
    public void Create_LowerCase_ConvertsToUpperCase()
    {
        // Act
        var code = CountryCode.Create("fr");

        // Assert
        code.Value.ShouldBe("FR");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_EmptyOrNull_ThrowsArgumentException(string? value)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => CountryCode.Create(value!));
    }

    [Theory]
    [InlineData("D")]
    [InlineData("DEU")]
    [InlineData("12")]
    [InlineData("D1")]
    public void Create_InvalidFormat_ThrowsArgumentException(string value)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => CountryCode.Create(value));
    }

    [Fact]
    public void Germany_HasCorrectValue()
    {
        CountryCode.Germany.Value.ShouldBe("DE");
    }

    [Fact]
    public void Austria_HasCorrectValue()
    {
        CountryCode.Austria.Value.ShouldBe("AT");
    }

    [Fact]
    public void Switzerland_HasCorrectValue()
    {
        CountryCode.Switzerland.Value.ShouldBe("CH");
    }

    [Fact]
    public void France_HasCorrectValue()
    {
        CountryCode.France.Value.ShouldBe("FR");
    }

    [Fact]
    public void IsEuMember_Germany_ReturnsTrue()
    {
        CountryCode.Germany.IsEuMember.ShouldBeTrue();
    }

    [Fact]
    public void IsEuMember_Switzerland_ReturnsFalse()
    {
        CountryCode.Switzerland.IsEuMember.ShouldBeFalse();
    }

    [Fact]
    public void IsEuMember_Norway_ReturnsFalse()
    {
        var norway = CountryCode.Create("NO");
        norway.IsEuMember.ShouldBeFalse();
    }

    [Fact]
    public void IsSchengenArea_Germany_ReturnsTrue()
    {
        CountryCode.Germany.IsSchengenArea.ShouldBeTrue();
    }

    [Fact]
    public void IsSchengenArea_Switzerland_ReturnsTrue()
    {
        CountryCode.Switzerland.IsSchengenArea.ShouldBeTrue();
    }

    [Fact]
    public void IsSchengenArea_UnitedKingdom_ReturnsFalse()
    {
        var uk = CountryCode.Create("GB");
        uk.IsSchengenArea.ShouldBeFalse();
    }

    [Fact]
    public void GetGermanName_Germany_ReturnsDeutschland()
    {
        CountryCode.Germany.GetGermanName().ShouldBe("Deutschland");
    }

    [Fact]
    public void GetGermanName_Austria_ReturnsOesterreich()
    {
        CountryCode.Austria.GetGermanName().ShouldBe("Österreich");
    }

    [Fact]
    public void GetGermanName_Switzerland_ReturnsSchweiz()
    {
        CountryCode.Switzerland.GetGermanName().ShouldBe("Schweiz");
    }

    [Fact]
    public void GetEnglishName_Germany_ReturnsGermany()
    {
        CountryCode.Germany.GetEnglishName().ShouldBe("Germany");
    }

    [Fact]
    public void GetEnglishName_Austria_ReturnsAustria()
    {
        CountryCode.Austria.GetEnglishName().ShouldBe("Austria");
    }

    [Fact]
    public void GetEnglishName_CzechRepublic_ReturnsCzechRepublic()
    {
        CountryCode.CzechRepublic.GetEnglishName().ShouldBe("Czech Republic");
    }

    [Fact]
    public void GetGermanName_UnknownCode_ReturnsCode()
    {
        var unknown = CountryCode.Create("XX");
        unknown.GetGermanName().ShouldBe("XX");
    }

    [Fact]
    public void GetEnglishName_UnknownCode_ReturnsCode()
    {
        var unknown = CountryCode.Create("XX");
        unknown.GetEnglishName().ShouldBe("XX");
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        CountryCode.Germany.ToString().ShouldBe("DE");
    }

    [Fact]
    public void Equality_SameCode_AreEqual()
    {
        var code1 = CountryCode.Create("DE");
        var code2 = CountryCode.Germany;

        code1.ShouldBe(code2);
    }

    [Fact]
    public void Equality_DifferentCodes_AreNotEqual()
    {
        var code1 = CountryCode.Germany;
        var code2 = CountryCode.Austria;

        code1.ShouldNotBe(code2);
    }
}

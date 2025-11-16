using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Domain.ValueObjects;

public class VehicleCategoryTests
{
    [Fact]
    public void Kleinwagen_ShouldHaveCorrectCodeAndName()
    {
        // Assert
        VehicleCategory.Kleinwagen.Code.ShouldBe("KLEIN");
        VehicleCategory.Kleinwagen.Name.ShouldBe("Kleinwagen");
    }

    [Fact]
    public void Kompaktklasse_ShouldHaveCorrectCodeAndName()
    {
        // Assert
        VehicleCategory.Kompaktklasse.Code.ShouldBe("KOMPAKT");
        VehicleCategory.Kompaktklasse.Name.ShouldBe("Kompaktklasse");
    }

    [Fact]
    public void Mittelklasse_ShouldHaveCorrectCodeAndName()
    {
        // Assert
        VehicleCategory.Mittelklasse.Code.ShouldBe("MITTEL");
        VehicleCategory.Mittelklasse.Name.ShouldBe("Mittelklasse");
    }

    [Fact]
    public void Oberklasse_ShouldHaveCorrectCodeAndName()
    {
        // Assert
        VehicleCategory.Oberklasse.Code.ShouldBe("OBER");
        VehicleCategory.Oberklasse.Name.ShouldBe("Oberklasse");
    }

    [Fact]
    public void SUV_ShouldHaveCorrectCodeAndName()
    {
        // Assert
        VehicleCategory.SUV.Code.ShouldBe("SUV");
        VehicleCategory.SUV.Name.ShouldBe("SUV");
    }

    [Fact]
    public void Kombi_ShouldHaveCorrectCodeAndName()
    {
        // Assert
        VehicleCategory.Kombi.Code.ShouldBe("KOMBI");
        VehicleCategory.Kombi.Name.ShouldBe("Kombi");
    }

    [Fact]
    public void Transporter_ShouldHaveCorrectCodeAndName()
    {
        // Assert
        VehicleCategory.Transporter.Code.ShouldBe("TRANS");
        VehicleCategory.Transporter.Name.ShouldBe("Transporter");
    }

    [Fact]
    public void Luxus_ShouldHaveCorrectCodeAndName()
    {
        // Assert
        VehicleCategory.Luxus.Code.ShouldBe("LUXUS");
        VehicleCategory.Luxus.Name.ShouldBe("Luxusklasse");
    }

    [Theory]
    [InlineData("KLEIN")]
    [InlineData("KOMPAKT")]
    [InlineData("MITTEL")]
    [InlineData("OBER")]
    [InlineData("SUV")]
    [InlineData("KOMBI")]
    [InlineData("TRANS")]
    [InlineData("LUXUS")]
    public void FromCode_WithValidCode_ShouldReturnCategory(string code)
    {
        // Act
        var category = VehicleCategory.FromCode(code);

        // Assert
        category.Code.ShouldBe(code);
    }

    [Theory]
    [InlineData("klein")] // Lowercase
    [InlineData("Klein")] // Mixed case
    [InlineData("KLEIN")] // Uppercase
    public void FromCode_ShouldBeCaseInsensitive(string code)
    {
        // Act
        var category = VehicleCategory.FromCode(code);

        // Assert
        category.ShouldBe(VehicleCategory.Kleinwagen);
    }

    [Fact]
    public void FromCode_WithInvalidCode_ShouldThrowArgumentException()
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => VehicleCategory.FromCode("INVALID"));
        ex.Message.ShouldContain("Unknown vehicle category code");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void FromCode_WithEmptyOrWhitespace_ShouldThrowArgumentException(string invalidCode)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => VehicleCategory.FromCode(invalidCode));
    }

    [Fact]
    public void FromCode_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => VehicleCategory.FromCode(null!));
    }

    [Fact]
    public void All_ShouldReturnAllCategories()
    {
        // Act
        var allCategories = VehicleCategory.All;

        // Assert
        allCategories.Count.ShouldBe(8);
        allCategories.ShouldContain(VehicleCategory.Kleinwagen);
        allCategories.ShouldContain(VehicleCategory.Kompaktklasse);
        allCategories.ShouldContain(VehicleCategory.Mittelklasse);
        allCategories.ShouldContain(VehicleCategory.Oberklasse);
        allCategories.ShouldContain(VehicleCategory.SUV);
        allCategories.ShouldContain(VehicleCategory.Kombi);
        allCategories.ShouldContain(VehicleCategory.Transporter);
        allCategories.ShouldContain(VehicleCategory.Luxus);
    }

    [Fact]
    public void ToString_ShouldReturnNameAndCode()
    {
        // Act
        var result = VehicleCategory.Kleinwagen.ToString();

        // Assert
        result.ShouldBe("Kleinwagen (KLEIN)");
    }

    [Fact]
    public void Equals_WithSameCategory_ShouldBeEqual()
    {
        // Arrange
        var category1 = VehicleCategory.FromCode("KLEIN");
        var category2 = VehicleCategory.Kleinwagen;

        // Act & Assert
        category1.ShouldBe(category2);
        (category1 == category2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCategories_ShouldNotBeEqual()
    {
        // Arrange
        var category1 = VehicleCategory.Kleinwagen;
        var category2 = VehicleCategory.Mittelklasse;

        // Act & Assert
        category1.ShouldNotBe(category2);
        (category1 != category2).ShouldBeTrue();
    }
}

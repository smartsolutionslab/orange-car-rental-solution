using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Domain.ValueObjects;

public class CompanyNameTests
{
    [Fact]
    public void Create_WithValidName_Succeeds()
    {
        // Act
        var companyName = CompanyName.Create("Orange Car Rental GmbH");

        // Assert
        companyName.Value.ShouldBe("Orange Car Rental GmbH");
    }

    [Fact]
    public void Create_WithWhitespace_TrimsValue()
    {
        // Act
        var companyName = CompanyName.Create("  Orange Car Rental GmbH  ");

        // Assert
        companyName.Value.ShouldBe("Orange Car Rental GmbH");
    }

    [Fact]
    public void Create_WithEmptyString_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => CompanyName.Create(""));
    }

    [Fact]
    public void Create_WithWhitespaceOnly_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => CompanyName.Create("   "));
    }

    [Fact]
    public void Create_WithTooShortName_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => CompanyName.Create("A")); // MinLength is 2
    }

    [Fact]
    public void Create_WithTooLongName_ThrowsArgumentException()
    {
        // Arrange
        var longName = new string('A', CompanyName.MaxLength + 1);

        // Act & Assert
        Should.Throw<ArgumentException>(() => CompanyName.Create(longName));
    }

    [Theory]
    [InlineData("Orange Car Rental GmbH", true)]
    [InlineData("Siemens AG", true)]
    [InlineData("Meyer & Söhne KG", true)]
    [InlineData("Schmidt OHG", true)]
    [InlineData("Startup UG", true)]
    [InlineData("GmbH & Co. KG Muster", true)]
    [InlineData("Deutsche Bahn SE", true)]
    [InlineData("Orange Car Rental Ltd.", true)]
    [InlineData("Orange Car Rental", false)]
    [InlineData("Some Company", false)]
    public void HasGermanLegalForm_ReturnsCorrectValue(string name, bool expected)
    {
        // Arrange
        var companyName = CompanyName.Create(name);

        // Act
        var hasLegalForm = companyName.HasGermanLegalForm();

        // Assert
        hasLegalForm.ShouldBe(expected);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        var companyName = CompanyName.Create("Orange Car Rental GmbH");

        // Act
        var result = companyName.ToString();

        // Assert
        result.ShouldBe("Orange Car Rental GmbH");
    }

    [Fact]
    public void ImplicitConversionToString_ReturnsValue()
    {
        // Arrange
        var companyName = CompanyName.Create("Orange Car Rental GmbH");

        // Act
        string result = companyName;

        // Assert
        result.ShouldBe("Orange Car Rental GmbH");
    }

    [Fact]
    public void Equality_SameValue_AreEqual()
    {
        // Arrange
        var name1 = CompanyName.Create("Orange Car Rental GmbH");
        var name2 = CompanyName.Create("Orange Car Rental GmbH");

        // Act & Assert
        name1.ShouldBe(name2);
    }
}

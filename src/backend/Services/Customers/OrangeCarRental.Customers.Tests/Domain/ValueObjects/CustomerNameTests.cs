using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Domain.ValueObjects;

public class CustomerNameTests
{
    [Fact]
    public void Of_WithValidFirstAndLastName_ShouldCreateCustomerName()
    {
        // Act
        var name = CustomerName.Of("Max", "Mustermann");

        // Assert
        name.FirstName.Value.ShouldBe("Max");
        name.LastName.Value.ShouldBe("Mustermann");
        name.Salutation.ShouldBeNull();
    }

    [Fact]
    public void Of_WithSalutation_ShouldIncludeSalutation()
    {
        // Act
        var name = CustomerName.Of("Max", "Mustermann", Salutation.Herr);

        // Assert
        name.FirstName.Value.ShouldBe("Max");
        name.LastName.Value.ShouldBe("Mustermann");
        name.Salutation.ShouldBe(Salutation.Herr);
    }

    [Fact]
    public void FullName_ShouldReturnFirstAndLastName()
    {
        // Arrange
        var name = CustomerName.Of("Max", "Mustermann");

        // Act
        var fullName = name.FullName;

        // Assert
        fullName.ShouldBe("Max Mustermann");
    }

    [Fact]
    public void FormalName_WithoutSalutation_ShouldReturnFullName()
    {
        // Arrange
        var name = CustomerName.Of("Max", "Mustermann");

        // Act
        var formalName = name.FormalName;

        // Assert
        formalName.ShouldBe("Max Mustermann");
    }

    [Theory]
    [InlineData(Salutation.Herr, "Herr Max Mustermann")]
    [InlineData(Salutation.Frau, "Frau Max Mustermann")]
    [InlineData(Salutation.Divers, "Divers Max Mustermann")]
    public void FormalName_WithSalutation_ShouldIncludeSalutation(Salutation salutation, string expected)
    {
        // Arrange
        var name = CustomerName.Of("Max", "Mustermann", salutation);

        // Act
        var formalName = name.FormalName;

        // Assert
        formalName.ShouldBe(expected);
    }

    [Fact]
    public void Anonymized_ShouldCreateAnonymizedName()
    {
        // Act
        var name = CustomerName.Anonymized();

        // Assert
        name.FirstName.Value.ShouldStartWith("[DELETED-");
        name.LastName.Value.ShouldStartWith("[DELETED-");
        name.Salutation.ShouldBeNull();
    }

    [Fact]
    public void ToString_ShouldReturnFormalName()
    {
        // Arrange
        var name = CustomerName.Of("Max", "Mustermann", Salutation.Herr);

        // Act
        var result = name.ToString();

        // Assert
        result.ShouldBe("Herr Max Mustermann");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var name1 = CustomerName.Of("Max", "Mustermann", Salutation.Herr);
        var name2 = CustomerName.Of("Max", "Mustermann", Salutation.Herr);

        // Act & Assert
        name1.ShouldBe(name2);
        (name1 == name2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentFirstNames_ShouldNotBeEqual()
    {
        // Arrange
        var name1 = CustomerName.Of("Max", "Mustermann");
        var name2 = CustomerName.Of("Anna", "Mustermann");

        // Act & Assert
        name1.ShouldNotBe(name2);
        (name1 != name2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentLastNames_ShouldNotBeEqual()
    {
        // Arrange
        var name1 = CustomerName.Of("Max", "Mustermann");
        var name2 = CustomerName.Of("Max", "Schmidt");

        // Act & Assert
        name1.ShouldNotBe(name2);
        (name1 != name2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WithDifferentSalutations_ShouldNotBeEqual()
    {
        // Arrange
        var name1 = CustomerName.Of("Max", "Mustermann", Salutation.Herr);
        var name2 = CustomerName.Of("Max", "Mustermann", Salutation.Frau);

        // Act & Assert
        name1.ShouldNotBe(name2);
    }

    [Fact]
    public void Of_WithValueObjects_ShouldCreateCustomerName()
    {
        // Arrange
        var firstName = FirstName.Of("Max");
        var lastName = LastName.Of("Mustermann");

        // Act
        var name = CustomerName.Of(firstName, lastName, Salutation.Herr);

        // Assert
        name.FirstName.ShouldBe(firstName);
        name.LastName.ShouldBe(lastName);
        name.Salutation.ShouldBe(Salutation.Herr);
    }
}

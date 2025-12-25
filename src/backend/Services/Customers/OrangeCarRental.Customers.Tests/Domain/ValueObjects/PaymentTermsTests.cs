using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Domain.ValueObjects;

public class PaymentTermsTests
{
    [Fact]
    public void Immediate_HasZeroDays()
    {
        // Assert
        PaymentTerms.Immediate.DaysUntilDue.ShouldBe(0);
        PaymentTerms.Immediate.IsImmediate.ShouldBeTrue();
    }

    [Fact]
    public void Net7_Has7Days()
    {
        // Assert
        PaymentTerms.Net7.DaysUntilDue.ShouldBe(7);
        PaymentTerms.Net7.IsImmediate.ShouldBeFalse();
    }

    [Fact]
    public void Net14_Has14Days()
    {
        // Assert
        PaymentTerms.Net14.DaysUntilDue.ShouldBe(14);
    }

    [Fact]
    public void Net30_Has30Days()
    {
        // Assert
        PaymentTerms.Net30.DaysUntilDue.ShouldBe(30);
    }

    [Fact]
    public void Net60_Has60Days()
    {
        // Assert
        PaymentTerms.Net60.DaysUntilDue.ShouldBe(60);
    }

    [Fact]
    public void Create_WithValidDays_Succeeds()
    {
        // Act
        var terms = PaymentTerms.Create(45);

        // Assert
        terms.DaysUntilDue.ShouldBe(45);
    }

    [Fact]
    public void Create_WithNegativeDays_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => PaymentTerms.Create(-1));
    }

    [Fact]
    public void Create_WithTooManyDays_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => PaymentTerms.Create(91)); // Max is 90
    }

    [Fact]
    public void CalculateDueDate_AddsCorrectDays()
    {
        // Arrange
        var invoiceDate = new DateOnly(2025, 1, 1);

        // Act & Assert
        PaymentTerms.Immediate.CalculateDueDate(invoiceDate).ShouldBe(new DateOnly(2025, 1, 1));
        PaymentTerms.Net14.CalculateDueDate(invoiceDate).ShouldBe(new DateOnly(2025, 1, 15));
        PaymentTerms.Net30.CalculateDueDate(invoiceDate).ShouldBe(new DateOnly(2025, 1, 31));
    }

    [Fact]
    public void GetGermanDisplayName_ReturnsCorrectName()
    {
        // Assert
        PaymentTerms.Immediate.GetGermanDisplayName().ShouldBe("Sofort fällig");
        PaymentTerms.Net7.GetGermanDisplayName().ShouldBe("Zahlungsziel 7 Tage");
        PaymentTerms.Net14.GetGermanDisplayName().ShouldBe("Zahlungsziel 14 Tage");
        PaymentTerms.Net30.GetGermanDisplayName().ShouldBe("Zahlungsziel 30 Tage");
        PaymentTerms.Net60.GetGermanDisplayName().ShouldBe("Zahlungsziel 60 Tage");
    }

    [Fact]
    public void GetEnglishDisplayName_ReturnsCorrectName()
    {
        // Assert
        PaymentTerms.Immediate.GetEnglishDisplayName().ShouldBe("Due immediately");
        PaymentTerms.Net14.GetEnglishDisplayName().ShouldBe("Net 14 days");
        PaymentTerms.Net30.GetEnglishDisplayName().ShouldBe("Net 30 days");
    }

    [Fact]
    public void GetGermanDisplayName_CustomDays_ReturnsFormattedName()
    {
        // Arrange
        var terms = PaymentTerms.Create(45);

        // Act
        var name = terms.GetGermanDisplayName();

        // Assert
        name.ShouldBe("Zahlungsziel 45 Tage");
    }

    [Fact]
    public void ToString_ReturnsEnglishDisplayName()
    {
        // Assert
        PaymentTerms.Net30.ToString().ShouldBe("Net 30 days");
    }

    [Fact]
    public void Equality_SameDays_AreEqual()
    {
        // Arrange
        var terms1 = PaymentTerms.Create(30);
        var terms2 = PaymentTerms.Net30;

        // Act & Assert
        terms1.ShouldBe(terms2);
    }
}

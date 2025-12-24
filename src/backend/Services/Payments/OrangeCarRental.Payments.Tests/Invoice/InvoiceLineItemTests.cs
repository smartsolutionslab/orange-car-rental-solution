using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Tests.Invoice;

public class InvoiceLineItemTests
{
    [Fact]
    public void ForVehicleRental_CreatesCorrectLineItem()
    {
        // Arrange
        var position = 1;
        var vehicleDescription = "VW Golf Kombi";
        var rentalDays = 5;
        var dailyRateNet = 50.00m;
        var pickupDate = new DateOnly(2025, 1, 15);
        var returnDate = new DateOnly(2025, 1, 20);

        // Act
        var lineItem = InvoiceLineItem.ForVehicleRental(
            position,
            vehicleDescription,
            rentalDays,
            dailyRateNet,
            pickupDate,
            returnDate);

        // Assert
        lineItem.Position.ShouldBe(1);
        lineItem.Description.ShouldBe("Fahrzeugmiete: VW Golf Kombi");
        lineItem.Quantity.ShouldBe(5);
        lineItem.Unit.ShouldBe("Tage");
        lineItem.UnitPriceNet.ShouldBe(50.00m);
        lineItem.VatRate.ShouldBe(0.19m);
        lineItem.ServicePeriodStart.ShouldBe(pickupDate);
        lineItem.ServicePeriodEnd.ShouldBe(returnDate);
    }

    [Fact]
    public void ForVehicleRental_SingleDay_UsesSingularUnit()
    {
        // Arrange & Act
        var lineItem = InvoiceLineItem.ForVehicleRental(
            1,
            "VW Golf",
            1, // Single day
            50.00m,
            new DateOnly(2025, 1, 15),
            new DateOnly(2025, 1, 15));

        // Assert
        lineItem.Unit.ShouldBe("Tag");
    }

    [Fact]
    public void TotalNet_CalculatesCorrectly()
    {
        // Arrange
        var lineItem = InvoiceLineItem.ForVehicleRental(
            1,
            "VW Golf",
            5,
            50.00m,
            new DateOnly(2025, 1, 15),
            new DateOnly(2025, 1, 20));

        // Act & Assert
        lineItem.TotalNet.ShouldBe(250.00m); // 5 * 50
    }

    [Fact]
    public void VatAmount_CalculatesCorrectly()
    {
        // Arrange
        var lineItem = InvoiceLineItem.ForVehicleRental(
            1,
            "VW Golf",
            5,
            50.00m,
            new DateOnly(2025, 1, 15),
            new DateOnly(2025, 1, 20));

        // Act & Assert
        lineItem.VatAmount.ShouldBe(47.50m); // 250 * 0.19 = 47.5
    }

    [Fact]
    public void TotalGross_CalculatesCorrectly()
    {
        // Arrange
        var lineItem = InvoiceLineItem.ForVehicleRental(
            1,
            "VW Golf",
            5,
            50.00m,
            new DateOnly(2025, 1, 15),
            new DateOnly(2025, 1, 20));

        // Act & Assert
        lineItem.TotalGross.ShouldBe(297.50m); // 250 + 47.5
    }

    [Fact]
    public void ForAdditionalService_CreatesCorrectLineItem()
    {
        // Arrange & Act
        var lineItem = InvoiceLineItem.ForAdditionalService(
            2,
            "GPS Navigation",
            5,
            "Tage",
            5.00m);

        // Assert
        lineItem.Position.ShouldBe(2);
        lineItem.Description.ShouldBe("GPS Navigation");
        lineItem.Quantity.ShouldBe(5);
        lineItem.Unit.ShouldBe("Tage");
        lineItem.UnitPriceNet.ShouldBe(5.00m);
        lineItem.TotalNet.ShouldBe(25.00m);
        lineItem.ServicePeriodStart.ShouldBeNull();
        lineItem.ServicePeriodEnd.ShouldBeNull();
    }

    [Fact]
    public void VatAmount_RoundsToTwoDecimals()
    {
        // Arrange - create a case where VAT would have more than 2 decimals
        var lineItem = new InvoiceLineItem
        {
            Position = 1,
            Description = "Test",
            Quantity = 1,
            Unit = "Stück",
            UnitPriceNet = 33.33m, // 33.33 * 0.19 = 6.3327
            VatRate = 0.19m
        };

        // Act & Assert
        lineItem.VatAmount.ShouldBe(6.33m); // Rounded
    }
}

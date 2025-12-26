using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
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
        lineItem.Quantity.Value.ShouldBe(5);
        lineItem.Quantity.Unit.ShouldBe("Tage");
        lineItem.UnitPrice.NetAmount.ShouldBe(50.00m);
        lineItem.VatRate.Value.ShouldBe(0.19m);
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
        lineItem.Quantity.Unit.ShouldBe("Tag");
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
            Quantity.Days(5),
            5.00m);

        // Assert
        lineItem.Position.ShouldBe(2);
        lineItem.Description.ShouldBe("GPS Navigation");
        lineItem.Quantity.Value.ShouldBe(5);
        lineItem.Quantity.Unit.ShouldBe("Tage");
        lineItem.UnitPrice.NetAmount.ShouldBe(5.00m);
        lineItem.TotalNet.ShouldBe(25.00m);
        lineItem.ServicePeriodStart.ShouldBeNull();
        lineItem.ServicePeriodEnd.ShouldBeNull();
    }

    [Fact]
    public void VatAmount_RoundsToTwoDecimals()
    {
        // Arrange - create a case where VAT would have more than 2 decimals
        var lineItem = InvoiceLineItem.ForAdditionalService(
            1,
            "Test",
            Quantity.Pieces(1),
            33.33m); // 33.33 * 0.19 = 6.3327

        // Act & Assert
        lineItem.VatAmount.ShouldBe(6.33m); // Rounded
    }

    [Fact]
    public void ForInsurance_CreatesCorrectLineItem()
    {
        // Arrange & Act
        var lineItem = InvoiceLineItem.ForInsurance(
            3,
            "Vollkasko ohne Selbstbeteiligung",
            5,
            15.00m);

        // Assert
        lineItem.Position.ShouldBe(3);
        lineItem.Description.ShouldBe("Versicherung: Vollkasko ohne Selbstbeteiligung");
        lineItem.Quantity.Value.ShouldBe(5);
        lineItem.Quantity.Unit.ShouldBe("Tage");
        lineItem.UnitPrice.NetAmount.ShouldBe(15.00m);
        lineItem.TotalNet.ShouldBe(75.00m);
    }

    [Fact]
    public void ForKilometerSurcharge_CreatesCorrectLineItem()
    {
        // Arrange & Act
        var lineItem = InvoiceLineItem.ForKilometerSurcharge(
            4,
            100,
            0.20m);

        // Assert
        lineItem.Position.ShouldBe(4);
        lineItem.Description.ShouldBe("Kilometerüberschreitung");
        lineItem.Quantity.Value.ShouldBe(100);
        lineItem.Quantity.Unit.ShouldBe("km");
        lineItem.UnitPrice.NetAmount.ShouldBe(0.20m);
        lineItem.TotalNet.ShouldBe(20.00m);
    }
}

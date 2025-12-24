using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Tests.Invoice;

public class InvoiceTests
{
    [Fact]
    public void Create_ValidData_CreatesInvoice()
    {
        // Arrange
        var invoiceNumber = InvoiceNumber.Create(1, 2025);
        var reservationId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var lineItems = new[]
        {
            InvoiceLineItem.ForVehicleRental(
                1, "VW Golf", 5, 50.00m,
                new DateOnly(2025, 1, 15),
                new DateOnly(2025, 1, 20))
        };

        // Act
        var invoice = Domain.Invoice.Invoice.Create(
            invoiceNumber: invoiceNumber,
            reservationId: reservationId,
            customerId: customerId,
            customerName: "Max Mustermann",
            customerStreet: "Musterstraße 123",
            customerPostalCode: "10115",
            customerCity: "Berlin",
            customerCountry: "Deutschland",
            customerVatId: null,
            lineItems: lineItems,
            serviceDate: new DateOnly(2025, 1, 20));

        // Assert
        invoice.Id.Value.ShouldNotBe(Guid.Empty);
        invoice.InvoiceNumber.ShouldBe(invoiceNumber);
        invoice.ReservationId.ShouldBe(reservationId);
        invoice.CustomerId.ShouldBe(customerId);
        invoice.CustomerName.ShouldBe("Max Mustermann");
        invoice.Status.ShouldBe(InvoiceStatus.Created);
        invoice.LineItems.Count.ShouldBe(1);
    }

    [Fact]
    public void Create_CalculatesTotalsCorrectly()
    {
        // Arrange
        var lineItems = new[]
        {
            InvoiceLineItem.ForVehicleRental(
                1, "VW Golf", 5, 50.00m,
                new DateOnly(2025, 1, 15),
                new DateOnly(2025, 1, 20)),
            InvoiceLineItem.ForAdditionalService(
                2, "GPS Navigation", 5, "Tage", 5.00m)
        };

        // Act
        var invoice = Domain.Invoice.Invoice.Create(
            invoiceNumber: InvoiceNumber.Create(1, 2025),
            reservationId: Guid.NewGuid(),
            customerId: Guid.NewGuid(),
            customerName: "Max Mustermann",
            customerStreet: "Musterstraße 123",
            customerPostalCode: "10115",
            customerCity: "Berlin",
            customerCountry: "Deutschland",
            customerVatId: null,
            lineItems: lineItems,
            serviceDate: new DateOnly(2025, 1, 20));

        // Assert
        invoice.TotalNet.ShouldBe(275.00m);  // 250 + 25
        invoice.TotalVat.ShouldBe(52.25m);   // 47.5 + 4.75
        invoice.TotalGross.ShouldBe(327.25m); // 297.5 + 29.75
    }

    [Fact]
    public void Create_SetsDefaultDueDate14DaysFromNow()
    {
        // Arrange & Act
        var invoice = Domain.Invoice.Invoice.Create(
            invoiceNumber: InvoiceNumber.Create(1, 2025),
            reservationId: Guid.NewGuid(),
            customerId: Guid.NewGuid(),
            customerName: "Max Mustermann",
            customerStreet: "Musterstraße 123",
            customerPostalCode: "10115",
            customerCity: "Berlin",
            customerCountry: "Deutschland",
            customerVatId: null,
            lineItems: [InvoiceLineItem.ForVehicleRental(1, "VW Golf", 1, 50m, DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow))],
            serviceDate: DateOnly.FromDateTime(DateTime.UtcNow));

        // Assert
        var expectedDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));
        invoice.DueDate.ShouldBe(expectedDueDate);
    }

    [Fact]
    public void Create_SetsCustomPaymentTerms()
    {
        // Arrange & Act
        var invoice = Domain.Invoice.Invoice.Create(
            invoiceNumber: InvoiceNumber.Create(1, 2025),
            reservationId: Guid.NewGuid(),
            customerId: Guid.NewGuid(),
            customerName: "Max Mustermann",
            customerStreet: "Musterstraße 123",
            customerPostalCode: "10115",
            customerCity: "Berlin",
            customerCountry: "Deutschland",
            customerVatId: null,
            lineItems: [InvoiceLineItem.ForVehicleRental(1, "VW Golf", 1, 50m, DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow))],
            serviceDate: DateOnly.FromDateTime(DateTime.UtcNow),
            paymentTermDays: 30);

        // Assert
        var expectedDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        invoice.DueDate.ShouldBe(expectedDueDate);
    }

    [Fact]
    public void MarkAsSent_FromCreated_ChangeStatus()
    {
        // Arrange
        var invoice = CreateTestInvoice();

        // Act
        var sentInvoice = invoice.MarkAsSent();

        // Assert
        sentInvoice.Status.ShouldBe(InvoiceStatus.Sent);
        sentInvoice.SentAt.ShouldNotBeNull();
    }

    [Fact]
    public void MarkAsSent_FromSent_ThrowsException()
    {
        // Arrange
        var invoice = CreateTestInvoice().MarkAsSent();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => invoice.MarkAsSent());
    }

    [Fact]
    public void MarkAsPaid_FromCreated_ChangesStatus()
    {
        // Arrange
        var invoice = CreateTestInvoice();

        // Act
        var paidInvoice = invoice.MarkAsPaid();

        // Assert
        paidInvoice.Status.ShouldBe(InvoiceStatus.Paid);
        paidInvoice.PaidAt.ShouldNotBeNull();
    }

    [Fact]
    public void MarkAsPaid_FromSent_ChangesStatus()
    {
        // Arrange
        var invoice = CreateTestInvoice().MarkAsSent();

        // Act
        var paidInvoice = invoice.MarkAsPaid();

        // Assert
        paidInvoice.Status.ShouldBe(InvoiceStatus.Paid);
    }

    [Fact]
    public void MarkAsPaid_FromVoided_ThrowsException()
    {
        // Arrange
        var invoice = CreateTestInvoice().Void();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => invoice.MarkAsPaid());
    }

    [Fact]
    public void Void_FromCreated_ChangesStatus()
    {
        // Arrange
        var invoice = CreateTestInvoice();

        // Act
        var voidedInvoice = invoice.Void();

        // Assert
        voidedInvoice.Status.ShouldBe(InvoiceStatus.Voided);
        voidedInvoice.VoidedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Void_FromPaid_ThrowsException()
    {
        // Arrange
        var invoice = CreateTestInvoice().MarkAsPaid();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => invoice.Void());
    }

    [Fact]
    public void WithPdfDocument_AttachesPdf()
    {
        // Arrange
        var invoice = CreateTestInvoice();
        var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // PDF magic number

        // Act
        var invoiceWithPdf = invoice.WithPdfDocument(pdfBytes);

        // Assert
        invoiceWithPdf.PdfDocument.ShouldBe(pdfBytes);
    }

    [Fact]
    public void Create_ContainsSellerInformation()
    {
        // Arrange & Act
        var invoice = CreateTestInvoice();

        // Assert
        invoice.SellerName.ShouldBe("Orange Car Rental GmbH");
        invoice.SellerStreet.ShouldNotBeNullOrEmpty();
        invoice.SellerPostalCode.ShouldNotBeNullOrEmpty();
        invoice.SellerCity.ShouldNotBeNullOrEmpty();
        invoice.VatId.ShouldStartWith("DE");
        invoice.TaxNumber.ShouldNotBeNullOrEmpty();
        invoice.TradeRegisterNumber.ShouldNotBeNullOrEmpty();
        invoice.ManagingDirector.ShouldNotBeNullOrEmpty();
    }

    private static Domain.Invoice.Invoice CreateTestInvoice()
    {
        return Domain.Invoice.Invoice.Create(
            invoiceNumber: InvoiceNumber.Create(1, 2025),
            reservationId: Guid.NewGuid(),
            customerId: Guid.NewGuid(),
            customerName: "Max Mustermann",
            customerStreet: "Musterstraße 123",
            customerPostalCode: "10115",
            customerCity: "Berlin",
            customerCountry: "Deutschland",
            customerVatId: null,
            lineItems: [
                InvoiceLineItem.ForVehicleRental(
                    1, "VW Golf", 5, 50.00m,
                    new DateOnly(2025, 1, 15),
                    new DateOnly(2025, 1, 20))
            ],
            serviceDate: new DateOnly(2025, 1, 20));
    }
}

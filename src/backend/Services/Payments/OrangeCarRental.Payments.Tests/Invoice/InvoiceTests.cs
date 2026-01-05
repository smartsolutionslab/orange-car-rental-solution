using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Tests.Invoice;

public class InvoiceTests
{
    [Fact]
    public void Create_ValidData_CreatesInvoice()
    {
        // Arrange
        var invoiceNumber = InvoiceNumber.Create(1, 2025);
        var reservationId = ReservationIdentifier.From(TestIds.Reservation1);
        var customerId = CustomerIdentifier.From(TestIds.Customer1);
        var (pickup, returnDate) = TestDates.RentalPeriod();
        var customer = CreateTestCustomer(customerId);
        var lineItems = new[]
        {
            InvoiceLineItem.ForVehicleRental(1, "VW Golf", 5, 50.00m, pickup, returnDate)
        };

        // Act
        var invoice = Domain.Invoice.Invoice.Create(
            invoiceNumber: invoiceNumber,
            reservationId: reservationId,
            customer: customer,
            lineItems: lineItems,
            serviceDate: returnDate);

        // Assert
        invoice.Id.Value.ShouldNotBe(Guid.Empty);
        invoice.InvoiceNumber.ShouldBe(invoiceNumber);
        invoice.ReservationIdentifier.Value.ShouldBe(reservationId.Value);
        invoice.Customer.CustomerIdentifier.Value.ShouldBe(customerId.Value);
        invoice.Customer.Name.ShouldBe(TestCustomer.MaxMustermann.FullName);
        invoice.Status.ShouldBe(InvoiceStatus.Created);
        invoice.LineItems.Count.ShouldBe(1);
    }

    [Fact]
    public void Create_CalculatesTotalsCorrectly()
    {
        // Arrange
        var (pickup, returnDate) = TestDates.RentalPeriod(startDaysFromNow: 7, rentalDays: 5);
        var lineItems = new[]
        {
            InvoiceLineItem.ForVehicleRental(1, "VW Golf", 5, 50.00m, pickup, returnDate),
            InvoiceLineItem.ForAdditionalService(
                2, "GPS Navigation",
                BuildingBlocks.Domain.ValueObjects.Quantity.Days(5),
                5.00m)
        };

        // Act
        var invoice = CreateTestInvoice(lineItems);

        // Assert
        invoice.TotalNet.ShouldBe(275.00m);  // 250 + 25
        invoice.TotalVat.ShouldBe(52.25m);   // 47.5 + 4.75
        invoice.TotalGross.ShouldBe(327.25m); // 297.5 + 29.75
    }

    [Fact]
    public void Create_SetsDefaultDueDate14DaysFromNow()
    {
        // Arrange & Act
        var invoice = CreateTestInvoice();

        // Assert
        var expectedDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));
        invoice.DueDate.ShouldBe(expectedDueDate);
    }

    [Fact]
    public void Create_SetsCustomPaymentTerms()
    {
        // Arrange
        var customer = CreateTestCustomer();
        var lineItems = new[] { CreateTestLineItem() };

        // Act
        var invoice = Domain.Invoice.Invoice.Create(
            invoiceNumber: InvoiceNumber.Create(1, 2025),
            reservationId: ReservationIdentifier.From(Guid.CreateVersion7()),
            customer: customer,
            lineItems: lineItems,
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
        invoice.Seller.CompanyName.ShouldBe("Orange Car Rental GmbH");
        invoice.Seller.Street.Value.ShouldNotBeNullOrEmpty();
        invoice.Seller.PostalCode.Value.ShouldNotBeNullOrEmpty();
        invoice.Seller.City.Value.ShouldNotBeNullOrEmpty();
        invoice.Seller.VatId.Value.ShouldStartWith("DE");
        invoice.Seller.TaxNumber.ShouldNotBeNullOrEmpty();
        invoice.Seller.TradeRegisterNumber.ShouldNotBeNullOrEmpty();
        invoice.Seller.ManagingDirector.Value.ShouldNotBeNullOrEmpty();
    }

    private static CustomerInvoiceInfo CreateTestCustomer(CustomerIdentifier? customerIdentifier = null)
    {
        return CustomerInvoiceInfo.Create(
            customerIdentifier: customerIdentifier ?? CustomerIdentifier.From(TestIds.Customer1),
            name: TestCustomer.MaxMustermann.FullName,
            street: TestCustomer.MaxMustermann.Street,
            postalCode: TestCustomer.MaxMustermann.PostalCode,
            city: TestCustomer.MaxMustermann.City,
            country: TestCustomer.MaxMustermann.Country);
    }

    private static InvoiceLineItem CreateTestLineItem()
    {
        var (pickup, returnDate) = TestDates.RentalPeriod(startDaysFromNow: 7, rentalDays: 5);
        return InvoiceLineItem.ForVehicleRental(1, "VW Golf", 5, 50.00m, pickup, returnDate);
    }

    private static Domain.Invoice.Invoice CreateTestInvoice(IEnumerable<InvoiceLineItem>? lineItems = null)
    {
        return Domain.Invoice.Invoice.Create(
            invoiceNumber: InvoiceNumber.Create(1, 2025),
            reservationId: ReservationIdentifier.From(TestIds.Reservation1),
            customer: CreateTestCustomer(),
            lineItems: lineItems ?? [CreateTestLineItem()],
            serviceDate: TestDates.DefaultReturn);
    }
}

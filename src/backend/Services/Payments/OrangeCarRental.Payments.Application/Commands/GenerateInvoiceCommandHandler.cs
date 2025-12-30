using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

/// <summary>
///     Handles the GenerateInvoiceCommand.
///     Creates a German-compliant invoice with PDF document.
/// </summary>
public sealed class GenerateInvoiceCommandHandler(
    IPaymentsUnitOfWork unitOfWork,
    IInvoiceGenerator invoiceGenerator)
    : ICommandHandler<GenerateInvoiceCommand, GenerateInvoiceResult>
{
    public async Task<GenerateInvoiceResult> HandleAsync(
        GenerateInvoiceCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if invoice already exists for this reservation
            var existingInvoice = await unitOfWork.Invoices
                .GetByReservationIdAsync(command.ReservationId, cancellationToken);

            if (existingInvoice != null)
            {
                return new GenerateInvoiceResult(
                    Success: true,
                    InvoiceId: existingInvoice.Id.Value,
                    InvoiceNumber: existingInvoice.InvoiceNumber.Value,
                    PdfDocument: existingInvoice.PdfDocument,
                    ErrorMessage: null);
            }

            // Generate consecutive invoice number
            var year = DateTime.UtcNow.Year;
            var nextSequence = await unitOfWork.Invoices
                .GetNextSequenceNumberAsync(year, cancellationToken);
            var invoiceNumber = InvoiceNumber.Create(nextSequence, year);

            // Create line item for vehicle rental
            var lineItem = InvoiceLineItem.ForVehicleRental(
                position: 1,
                vehicleDescription: command.VehicleDescription,
                rentalDays: command.RentalDays,
                dailyRateNet: command.DailyRate.NetAmount,
                pickupDate: command.PickupDate,
                returnDate: command.ReturnDate);

            // Create customer invoice info
            var customerInfo = CustomerInvoiceInfo.Create(
                customerId: command.CustomerId,
                name: command.CustomerName,
                street: command.CustomerStreet.Value,
                postalCode: command.CustomerPostalCode.Value,
                city: command.CustomerCity.Value,
                country: command.CustomerCountry.Value,
                vatId: command.CustomerVatId?.Value);

            // Create invoice
            var invoice = Invoice.Create(
                invoiceNumber: invoiceNumber,
                reservationId: command.ReservationId,
                customer: customerInfo,
                lineItems: [lineItem],
                serviceDate: command.ReturnDate,
                paymentTermDays: 14,
                paymentId: command.PaymentId);

            // Generate PDF
            var pdfBytes = invoiceGenerator.GeneratePdf(invoice);
            invoice = invoice.WithPdfDocument(pdfBytes);

            // Persist
            await unitOfWork.Invoices.AddAsync(invoice, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new GenerateInvoiceResult(
                Success: true,
                InvoiceId: invoice.Id.Value,
                InvoiceNumber: invoiceNumber.Value,
                PdfDocument: pdfBytes,
                ErrorMessage: null);
        }
        catch (Exception ex)
        {
            return new GenerateInvoiceResult(
                Success: false,
                InvoiceId: null,
                InvoiceNumber: null,
                PdfDocument: null,
                ErrorMessage: ex.Message);
        }
    }
}

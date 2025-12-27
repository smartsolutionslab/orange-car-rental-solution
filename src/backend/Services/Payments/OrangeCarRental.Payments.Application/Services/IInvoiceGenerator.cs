using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;

/// <summary>
///     Service for generating PDF invoices.
/// </summary>
public interface IInvoiceGenerator
{
    /// <summary>
    ///     Generates a PDF document for the given invoice.
    /// </summary>
    /// <param name="invoice">The invoice to generate PDF for.</param>
    /// <returns>PDF document as byte array.</returns>
    byte[] GeneratePdf(Invoice invoice);
}

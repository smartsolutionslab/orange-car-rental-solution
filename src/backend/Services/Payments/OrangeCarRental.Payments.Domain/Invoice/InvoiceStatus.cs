namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

/// <summary>
///     Status of an invoice.
/// </summary>
public enum InvoiceStatus
{
    /// <summary>
    ///     Invoice has been created/generated.
    /// </summary>
    Created = 0,

    /// <summary>
    ///     Invoice has been sent to the customer.
    /// </summary>
    Sent = 1,

    /// <summary>
    ///     Invoice has been paid.
    /// </summary>
    Paid = 2,

    /// <summary>
    ///     Invoice has been voided/cancelled.
    /// </summary>
    Voided = 3,

    /// <summary>
    ///     Invoice is overdue (not paid within terms).
    /// </summary>
    Overdue = 4
}

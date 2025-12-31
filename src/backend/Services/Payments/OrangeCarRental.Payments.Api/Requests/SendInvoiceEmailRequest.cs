namespace SmartSolutionsLab.OrangeCarRental.Payments.Api.Requests;

/// <summary>
///     Request to send invoice via email.
/// </summary>
/// <param name="RecipientEmail">Customer's email address.</param>
public sealed record SendInvoiceEmailRequest(string RecipientEmail);

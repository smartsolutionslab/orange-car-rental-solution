namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Services;

/// <summary>
///     Represents an email attachment.
/// </summary>
/// <param name="FileName">The file name including extension.</param>
/// <param name="Content">The file content as bytes.</param>
/// <param name="ContentType">The MIME content type.</param>
public sealed record EmailAttachment(
    string FileName,
    byte[] Content,
    string ContentType);

/// <summary>
///     Service for sending email notifications.
/// </summary>
public interface IEmailService
{
    /// <summary>
    ///     Sends an email notification.
    /// </summary>
    /// <param name="toEmail">Recipient email address.</param>
    /// <param name="subject">Email subject.</param>
    /// <param name="body">Email body (can be HTML).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Provider message ID if successful.</returns>
    Task<string> SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sends an email notification with attachments.
    /// </summary>
    /// <param name="toEmail">Recipient email address.</param>
    /// <param name="subject">Email subject.</param>
    /// <param name="body">Email body (can be HTML).</param>
    /// <param name="attachments">List of attachments to include.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Provider message ID if successful.</returns>
    Task<string> SendEmailWithAttachmentsAsync(
        string toEmail,
        string subject,
        string body,
        IReadOnlyList<EmailAttachment> attachments,
        CancellationToken cancellationToken = default);
}

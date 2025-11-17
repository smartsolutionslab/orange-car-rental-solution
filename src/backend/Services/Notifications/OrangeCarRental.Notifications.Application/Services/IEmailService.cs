namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Services;

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
}

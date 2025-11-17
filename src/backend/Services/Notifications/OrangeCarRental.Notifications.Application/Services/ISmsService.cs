namespace SmartSolutionsLab.OrangeCarRental.Notifications.Application.Services;

/// <summary>
///     Service for sending SMS notifications.
/// </summary>
public interface ISmsService
{
    /// <summary>
    ///     Sends an SMS notification.
    /// </summary>
    /// <param name="toPhone">Recipient phone number.</param>
    /// <param name="message">SMS message text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Provider message ID if successful.</returns>
    Task<string> SendSmsAsync(
        string toPhone,
        string message,
        CancellationToken cancellationToken = default);
}

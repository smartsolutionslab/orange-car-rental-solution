namespace SmartSolutionsLab.OrangeCarRental.Notifications.Api.Requests;

/// <summary>
///     Request DTO for sending an email notification.
/// </summary>
public sealed record SendEmailRequest(
    string RecipientEmail,
    string Subject,
    string Body);

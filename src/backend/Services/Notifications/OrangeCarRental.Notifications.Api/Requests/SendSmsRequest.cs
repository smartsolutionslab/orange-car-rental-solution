namespace SmartSolutionsLab.OrangeCarRental.Notifications.Api.Requests;

/// <summary>
///     Request DTO for sending an SMS notification.
/// </summary>
public sealed record SendSmsRequest(
    string RecipientPhone,
    string Message);

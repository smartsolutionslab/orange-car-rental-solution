namespace SmartSolutionsLab.OrangeCarRental.Payments.Api.Shared;

/// <summary>
///     Result of sending invoice email.
/// </summary>
/// <param name="Success">Whether the email was sent successfully.</param>
/// <param name="ProviderMessageId">Message ID from the email provider.</param>
/// <param name="Message">Success or error message.</param>
public sealed record SendEmailResultDto(
    bool Success,
    string? ProviderMessageId,
    string Message);

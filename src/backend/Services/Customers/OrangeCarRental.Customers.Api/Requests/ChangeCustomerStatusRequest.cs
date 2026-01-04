namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Requests;

/// <summary>
///     Request DTO for changing customer status.
///     Accepts primitives from HTTP requests and maps to ChangeCustomerStatusCommand with value objects.
/// </summary>
public sealed record ChangeCustomerStatusRequest(
    string NewStatus,
    string Reason);

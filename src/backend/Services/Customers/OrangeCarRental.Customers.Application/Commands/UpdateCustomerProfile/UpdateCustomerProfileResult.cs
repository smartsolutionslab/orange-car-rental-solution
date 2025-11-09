using System.Diagnostics.CodeAnalysis;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;

/// <summary>
///     Result of customer profile update operation.
/// </summary>
[method: SetsRequiredMembers]
public sealed record UpdateCustomerProfileResult(
    Guid CustomerIdentifier,
    bool Success,
    string Message,
    DateTime UpdatedAtUtc);

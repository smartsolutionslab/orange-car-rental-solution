using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

/// <summary>
///     Event raised when a customer is upgraded to a business account.
/// </summary>
/// <param name="CustomerId">The customer identifier.</param>
/// <param name="CompanyName">The company name.</param>
/// <param name="VATId">The German VAT ID (USt-IdNr.).</param>
/// <param name="PaymentTerms">The agreed payment terms.</param>
/// <param name="UpgradedAtUtc">When the upgrade occurred.</param>
public sealed record CustomerUpgradedToBusiness(
    CustomerIdentifier CustomerId,
    CompanyName CompanyName,
    VATId VATId,
    PaymentTerms PaymentTerms,
    DateTime UpgradedAtUtc) : DomainEvent;

using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

/// <summary>
///     Event raised when a business customer's details are updated.
/// </summary>
/// <param name="CustomerId">The customer identifier.</param>
/// <param name="CompanyName">The updated company name.</param>
/// <param name="VATId">The updated VAT ID.</param>
/// <param name="PaymentTerms">The updated payment terms.</param>
/// <param name="UpdatedAtUtc">When the update occurred.</param>
public sealed record BusinessDetailsUpdated(
    CustomerIdentifier CustomerId,
    CompanyName CompanyName,
    VATId VATId,
    PaymentTerms PaymentTerms,
    DateTime UpdatedAtUtc) : DomainEvent;

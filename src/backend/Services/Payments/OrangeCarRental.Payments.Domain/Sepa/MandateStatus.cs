namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

/// <summary>
///     Status of a SEPA mandate.
/// </summary>
public enum MandateStatus
{
    /// <summary>
    ///     Mandate is active and can be used for direct debits.
    /// </summary>
    Active = 1,

    /// <summary>
    ///     Mandate has been revoked by the customer.
    /// </summary>
    Revoked = 2,

    /// <summary>
    ///     Mandate has expired (no use for 36 months).
    /// </summary>
    Expired = 3
}

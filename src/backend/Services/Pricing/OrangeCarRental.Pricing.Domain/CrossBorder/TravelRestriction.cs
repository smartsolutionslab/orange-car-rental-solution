namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.CrossBorder;

/// <summary>
///     Cross-border travel restriction level.
/// </summary>
public enum TravelRestriction
{
    /// <summary>
    ///     Travel is allowed without restrictions.
    /// </summary>
    Allowed = 1,

    /// <summary>
    ///     Travel requires advance permit/notification.
    /// </summary>
    PermitRequired = 2,

    /// <summary>
    ///     Travel is restricted - special approval needed.
    /// </summary>
    Restricted = 3,

    /// <summary>
    ///     Travel is prohibited - not allowed under any circumstances.
    /// </summary>
    Prohibited = 4
}

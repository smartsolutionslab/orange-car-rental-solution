namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.KilometerPackage;

/// <summary>
///     Types of kilometer packages available for German car rentals.
/// </summary>
public enum KilometerPackageType
{
    /// <summary>
    ///     100 km per day included.
    /// </summary>
    Limited100 = 1,

    /// <summary>
    ///     200 km per day included.
    /// </summary>
    Limited200 = 2,

    /// <summary>
    ///     Unlimited kilometers.
    /// </summary>
    Unlimited = 3
}

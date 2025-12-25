namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.InsurancePackage;

/// <summary>
///     Types of insurance coverage available for German car rentals.
/// </summary>
public enum InsuranceType
{
    /// <summary>
    ///     Liability insurance only (Haftpflichtversicherung).
    ///     Mandatory minimum coverage - covers damage to third parties.
    /// </summary>
    Haftpflicht = 1,

    /// <summary>
    ///     Partial coverage (Teilkasko).
    ///     Covers theft, fire, glass damage, natural disasters, and animal collisions.
    /// </summary>
    Teilkasko = 2,

    /// <summary>
    ///     Comprehensive coverage (Vollkasko).
    ///     Includes Teilkasko plus own damage from accidents.
    /// </summary>
    Vollkasko = 3,

    /// <summary>
    ///     Comprehensive coverage with zero deductible (Vollkasko ohne Selbstbeteiligung).
    ///     Premium option with no out-of-pocket costs.
    /// </summary>
    VollkaskoZeroDeductible = 4
}

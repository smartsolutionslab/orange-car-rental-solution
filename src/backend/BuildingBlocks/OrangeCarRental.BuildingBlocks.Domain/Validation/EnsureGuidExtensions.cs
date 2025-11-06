namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
///     Provides fluent validation extension methods for Guid values.
/// </summary>
public static class EnsureGuidExtensions
{
    /// <summary>
    ///     Ensures the Guid value is not empty (Guid.Empty).
    /// </summary>
    public static Ensurer<Guid> IsNotEmpty(this Ensurer<Guid> ensurer)
    {
        if (ensurer.Value == Guid.Empty)
            throw new ArgumentException("GUID cannot be empty.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    ///     Ensures the nullable Guid value is not null and not empty.
    /// </summary>
    public static Ensurer<Guid?> IsNotNullOrEmpty(this Ensurer<Guid?> ensurer)
    {
        if (ensurer.Value is null)
            throw new ArgumentNullException(ensurer.ParameterName);

        if (ensurer.Value == Guid.Empty)
            throw new ArgumentException("GUID cannot be empty.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    ///     Ensures the Guid value is empty (Guid.Empty).
    /// </summary>
    public static Ensurer<Guid> IsEmpty(this Ensurer<Guid> ensurer)
    {
        if (ensurer.Value != Guid.Empty)
            throw new ArgumentException("GUID must be empty.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    ///     Ensures the Guid is one of the specified valid values.
    /// </summary>
    public static Ensurer<Guid> AndIsOneOf(this Ensurer<Guid> ensurer, params Guid[] validValues)
    {
        if (!validValues.Contains(ensurer.Value))
            throw new ArgumentException(
                $"GUID must be one of the valid values: {string.Join(", ", validValues)}",
                ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    ///     Ensures the Guid is not one of the specified invalid values.
    /// </summary>
    public static Ensurer<Guid> AndIsNotOneOf(this Ensurer<Guid> ensurer, params Guid[] invalidValues)
    {
        if (invalidValues.Contains(ensurer.Value))
            throw new ArgumentException(
                $"GUID must not be one of the invalid values: {string.Join(", ", invalidValues)}",
                ensurer.ParameterName);

        return ensurer;
    }
}

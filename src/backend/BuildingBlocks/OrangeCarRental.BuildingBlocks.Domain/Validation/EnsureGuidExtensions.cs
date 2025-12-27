namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
///     Provides fluent validation extension methods for Guid values.
/// </summary>
public static class EnsureGuidExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;Guid&gt;.
    /// </summary>
    extension(Ensurer<Guid> ensurer)
    {
        /// <summary>
        ///     Ensures the Guid value is not empty (Guid.Empty).
        /// </summary>
        public Ensurer<Guid> IsNotEmpty()
        {
            if (ensurer.Value == Guid.Empty)
                throw new ArgumentException("GUID cannot be empty.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the Guid value is empty (Guid.Empty).
        /// </summary>
        public Ensurer<Guid> IsEmpty()
        {
            if (ensurer.Value != Guid.Empty)
                throw new ArgumentException("GUID must be empty.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the Guid is one of the specified valid values.
        ///     Uses C# 14 params ReadOnlySpan for zero-allocation parameter passing.
        /// </summary>
        public Ensurer<Guid> AndIsOneOf(params ReadOnlySpan<Guid> validValues)
        {
            if (!validValues.Contains(ensurer.Value))
                throw new ArgumentException(
                    $"GUID must be one of the valid values: {string.Join(", ", validValues.ToArray())}",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the Guid is not one of the specified invalid values.
        ///     Uses C# 14 params ReadOnlySpan for zero-allocation parameter passing.
        /// </summary>
        public Ensurer<Guid> AndIsNotOneOf(params ReadOnlySpan<Guid> invalidValues)
        {
            if (invalidValues.Contains(ensurer.Value))
                throw new ArgumentException(
                    $"GUID must not be one of the invalid values: {string.Join(", ", invalidValues.ToArray())}",
                    ensurer.ParameterName);

            return ensurer;
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;Guid?&gt;.
    /// </summary>
    extension(Ensurer<Guid?> ensurer)
    {
        /// <summary>
        ///     Ensures the nullable Guid value is not null and not empty.
        /// </summary>
        public Ensurer<Guid?> IsNotNullOrEmpty()
        {
            if (ensurer.Value is null)
                throw new ArgumentNullException(ensurer.ParameterName);

            if (ensurer.Value == Guid.Empty)
                throw new ArgumentException("GUID cannot be empty.", ensurer.ParameterName);

            return ensurer;
        }
    }
}

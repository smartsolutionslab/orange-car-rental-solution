namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;

/// <summary>
/// Provides consistent test IDs for cross-service testing.
/// Use these when you need stable, predictable IDs across tests.
/// </summary>
public static class TestIds
{
    /// <summary>
    /// Creates a new unique ID using Guid v7 (time-ordered).
    /// </summary>
    public static Guid New() => Guid.CreateVersion7();

    /// <summary>
    /// A fixed customer ID for tests that need a stable reference.
    /// </summary>
    public static readonly Guid Customer1 = Guid.Parse("01234567-0123-0123-0123-012345670001");

    /// <summary>
    /// A second fixed customer ID.
    /// </summary>
    public static readonly Guid Customer2 = Guid.Parse("01234567-0123-0123-0123-012345670002");

    /// <summary>
    /// A fixed vehicle ID for tests.
    /// </summary>
    public static readonly Guid Vehicle1 = Guid.Parse("01234567-0123-0123-0123-012345670101");

    /// <summary>
    /// A second fixed vehicle ID.
    /// </summary>
    public static readonly Guid Vehicle2 = Guid.Parse("01234567-0123-0123-0123-012345670102");

    /// <summary>
    /// A fixed reservation ID for tests.
    /// </summary>
    public static readonly Guid Reservation1 = Guid.Parse("01234567-0123-0123-0123-012345670201");

    /// <summary>
    /// A second fixed reservation ID.
    /// </summary>
    public static readonly Guid Reservation2 = Guid.Parse("01234567-0123-0123-0123-012345670202");

    /// <summary>
    /// A fixed payment ID for tests.
    /// </summary>
    public static readonly Guid Payment1 = Guid.Parse("01234567-0123-0123-0123-012345670301");

    /// <summary>
    /// A fixed invoice ID for tests.
    /// </summary>
    public static readonly Guid Invoice1 = Guid.Parse("01234567-0123-0123-0123-012345670401");

    /// <summary>
    /// A fixed location ID for tests.
    /// </summary>
    public static readonly Guid Location1 = Guid.Parse("01234567-0123-0123-0123-012345670501");
}

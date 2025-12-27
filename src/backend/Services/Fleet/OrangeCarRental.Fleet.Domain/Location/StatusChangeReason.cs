using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

/// <summary>
///     Reason for a location status change.
///     Represents why a location's status was changed (e.g., "Scheduled maintenance", "Temporary closure due to weather").
/// </summary>
/// <param name="Value">The reason text value.</param>
public readonly record struct StatusChangeReason(string Value) : IValueObject
{
    public static StatusChangeReason From(string reason)
    {
        var trimmed = reason?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(reason))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(500);

        return new StatusChangeReason(trimmed);
    }

    public static StatusChangeReason? FromNullable(string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) return null;

        return From(reason);
    }

    public static implicit operator string?(StatusChangeReason? reason) => reason?.Value;

    public override string ToString() => Value;
}

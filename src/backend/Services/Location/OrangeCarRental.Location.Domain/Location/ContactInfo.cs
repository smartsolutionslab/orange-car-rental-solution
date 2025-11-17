using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Contact information for a rental location.
/// </summary>
public readonly record struct ContactInfo(string Phone, string Email) : IValueObject
{
    public static ContactInfo Of(string phone, string email)
    {
        Ensure.That(phone, nameof(phone))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(20);

        Ensure.That(email, nameof(email))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(255)
            .AndSatisfies(
                v => v.Contains('@') && v.Contains('.'),
                "Email must be in valid format");

        return new ContactInfo(phone.Trim(), email.ToLowerInvariant().Trim());
    }

    public override string ToString() => $"{Phone}, {Email}";
}

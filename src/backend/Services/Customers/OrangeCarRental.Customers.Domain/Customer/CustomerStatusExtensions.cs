using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

public static class CustomerStatusExtensions
{
    public static CustomerStatus Parse(this string value)
    {
        Ensure.That(value, nameof(value))
            .ThrowIf(!Enum.TryParse<CustomerStatus>(value, true, out var newStatus),
                $"Invalid customer status: '{value}'. Valid values are: Active, Suspended, Blocked.");

        return Enum.Parse<CustomerStatus>(value, true);
    }

    public static CustomerStatus? TryParse(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        Enum.TryParse<CustomerStatus>(value, true, out var parsedStatus);
        return parsedStatus;
    }
}

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

public static class CustomerStatusExtensions
{
    public static CustomerStatus Parse(this string value)
    {
        if (!Enum.TryParse<CustomerStatus>(value, true, out var newStatus))
        {
            throw new ArgumentException($"Invalid customer status: '{value}'. Valid values are: Active, Suspended, Blocked.",
                nameof(value));
        }

        return newStatus;
    }
}

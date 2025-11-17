using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Builders;

/// <summary>
/// Test data builder for Customer aggregates.
/// Uses fluent API with sensible defaults.
/// </summary>
public class CustomerBuilder
{
    private CustomerName _name = CustomerName.Of("Max", "Mustermann", Salutation.Herr);
    private Email _email = Email.Of("max.mustermann@example.com");
    private PhoneNumber _phone = PhoneNumber.Of("0151 12345678");
    private BirthDate _birthDate = BirthDate.Of(new DateOnly(1990, 1, 1)); // 35 years old
    private Address _address = Address.Of("Hauptstraße 123", "Berlin", "10115", "Deutschland");
    private DriversLicense _license;

    public CustomerBuilder()
    {
        // Default license: issued 5 years ago, expires in 5 years
        var issueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
        var expiryDate = DateOnly.FromDateTime(DateTime.Now.AddYears(5));
        _license = DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate);
    }

    /// <summary>
    /// Sets the customer name.
    /// </summary>
    public CustomerBuilder WithName(string firstName, string lastName, Salutation? salutation = null)
    {
        _name = salutation.HasValue
            ? CustomerName.Of(firstName, lastName, salutation.Value)
            : CustomerName.Of(firstName, lastName);
        return this;
    }

    /// <summary>
    /// Sets the customer as male (Herr).
    /// </summary>
    public CustomerBuilder AsMale(string firstName = "Max", string lastName = "Mustermann")
    {
        _name = CustomerName.Of(firstName, lastName, Salutation.Herr);
        return this;
    }

    /// <summary>
    /// Sets the customer as female (Frau).
    /// </summary>
    public CustomerBuilder AsFemale(string firstName = "Anna", string lastName = "Schmidt")
    {
        _name = CustomerName.Of(firstName, lastName, Salutation.Frau);
        return this;
    }

    /// <summary>
    /// Sets the email address.
    /// </summary>
    public CustomerBuilder WithEmail(string email)
    {
        _email = Email.Of(email);
        return this;
    }

    /// <summary>
    /// Sets the phone number.
    /// </summary>
    public CustomerBuilder WithPhone(string phone)
    {
        _phone = PhoneNumber.Of(phone);
        return this;
    }

    /// <summary>
    /// Sets the birth date.
    /// </summary>
    public CustomerBuilder WithBirthDate(DateOnly birthDate)
    {
        _birthDate = BirthDate.Of(birthDate);
        return this;
    }

    /// <summary>
    /// Sets the age in years.
    /// Automatically adjusts license dates to be valid for the given age.
    /// </summary>
    public CustomerBuilder WithAge(int years)
    {
        var birthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-years));
        _birthDate = BirthDate.Of(birthDate);

        // Adjust license dates to be valid for the age
        // License should be issued after the person turned 18
        var earliestLicenseDate = birthDate.AddYears(18);
        var now = DateOnly.FromDateTime(DateTime.Now);

        // If person is 18 or older, issue license 1 year after they turned 18 (or 1 year ago if younger than 19)
        var issueDate = years >= 19
            ? earliestLicenseDate.AddYears(1)
            : now.AddYears(-1);

        // Ensure issue date is not in the future
        if (issueDate > now)
        {
            issueDate = now.AddDays(-30); // Issued 30 days ago
        }

        var expiryDate = DateOnly.FromDateTime(DateTime.Now.AddYears(5));
        _license = DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate);

        return this;
    }

    /// <summary>
    /// Sets the customer to be 17 years old (under minimum age).
    /// </summary>
    public CustomerBuilder UnderAge()
    {
        return WithAge(17);
    }

    /// <summary>
    /// Sets the customer to be exactly 18 years old (minimum age).
    /// </summary>
    public CustomerBuilder ExactlyMinimumAge()
    {
        return WithAge(18);
    }

    /// <summary>
    /// Sets the address.
    /// </summary>
    public CustomerBuilder WithAddress(string street, string city, string postalCode, string country = "Deutschland")
    {
        _address = Address.Of(street, city, postalCode, country);
        return this;
    }

    /// <summary>
    /// Sets the address in Berlin.
    /// </summary>
    public CustomerBuilder InBerlin(string street = "Hauptstraße 123", string postalCode = "10115")
    {
        _address = Address.Of(street, "Berlin", postalCode, "Deutschland");
        return this;
    }

    /// <summary>
    /// Sets the address in Munich.
    /// </summary>
    public CustomerBuilder InMunich(string street = "Marienplatz 1", string postalCode = "80331")
    {
        _address = Address.Of(street, "München", postalCode, "Deutschland");
        return this;
    }

    /// <summary>
    /// Sets the driver's license.
    /// </summary>
    public CustomerBuilder WithLicense(string number, string country, DateOnly issueDate, DateOnly expiryDate)
    {
        _license = DriversLicense.Of(number, country, issueDate, expiryDate);
        return this;
    }

    /// <summary>
    /// Sets the license to be expired (yesterday).
    /// </summary>
    public CustomerBuilder WithExpiredLicense()
    {
        var issueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-10));
        var expiryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
        _license = DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate);
        return this;
    }

    /// <summary>
    /// Sets the license to expire in the specified number of days.
    /// </summary>
    public CustomerBuilder WithLicenseExpiringIn(int days)
    {
        var issueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
        var expiryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(days));
        _license = DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate);
        return this;
    }

    /// <summary>
    /// Sets the license to expire in less than 30 days (invalid).
    /// </summary>
    public CustomerBuilder WithLicenseExpiringSoon()
    {
        return WithLicenseExpiringIn(15); // 15 days < 30 days minimum
    }

    /// <summary>
    /// Sets the license to expire in exactly 30 days (edge case, valid).
    /// </summary>
    public CustomerBuilder WithLicenseExpiringInExactly30Days()
    {
        return WithLicenseExpiringIn(30);
    }

    /// <summary>
    /// Builds a Customer in Active status.
    /// </summary>
    public Customer Build()
    {
        return Customer.Register(
            _name,
            _email,
            _phone,
            _birthDate,
            _address,
            _license);
    }

    /// <summary>
    /// Creates a new CustomerBuilder with default values.
    /// </summary>
    public static CustomerBuilder Default() => new();

    /// <summary>
    /// Creates Max Mustermann (default German male customer).
    /// </summary>
    public static CustomerBuilder MaxMustermann() => new CustomerBuilder()
        .AsMale("Max", "Mustermann")
        .WithEmail("max.mustermann@example.com")
        .WithPhone("0151 12345678")
        .WithAge(35)
        .InBerlin();

    /// <summary>
    /// Creates Anna Schmidt (default German female customer).
    /// </summary>
    public static CustomerBuilder AnnaSchmidt() => new CustomerBuilder()
        .AsFemale("Anna", "Schmidt")
        .WithEmail("anna.schmidt@example.com")
        .WithPhone("0160 98765432")
        .WithAge(28)
        .InMunich();

    /// <summary>
    /// Creates a customer at minimum age (18).
    /// </summary>
    public static CustomerBuilder YoungCustomer() => new CustomerBuilder()
        .WithAge(18)
        .WithName("Lisa", "Müller", Salutation.Frau)
        .WithEmail("lisa.mueller@example.com");

    /// <summary>
    /// Creates a senior customer (65+).
    /// </summary>
    public static CustomerBuilder SeniorCustomer() => new CustomerBuilder()
        .WithAge(68)
        .WithName("Hans", "Weber", Salutation.Herr)
        .WithEmail("hans.weber@example.com");
}

using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Domain.Entities;

public class CustomerTests
{
    private readonly CustomerName _validName = CustomerName.Of("Max", "Mustermann", Salutation.Herr);
    private readonly Email _validEmail = Email.Of("max.mustermann@example.com");
    private readonly PhoneNumber _validPhone = PhoneNumber.Of("0151 12345678");
    private readonly BirthDate _validBirthDate = BirthDate.Of(new DateOnly(1990, 1, 1)); // 35 years old in 2025
    private readonly Address _validAddress = Address.Of("Hauptstra\u00DFe", "123", "Berlin", PostalCode.Of("10115"), City.Of("Berlin"), "Deutschland");
    private readonly DriversLicense _validLicense;

    public CustomerTests()
    {
        // License issued 5 years ago, expires in 5 years
        var issueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
        var expiryDate = DateOnly.FromDateTime(DateTime.Now.AddYears(5));
        _validLicense = DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate);
    }

    [Fact]
    public void Register_WithValidData_ShouldCreateCustomer()
    {
        // Act
        var customer = Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            _validBirthDate,
            _validAddress,
            _validLicense);

        // Assert
        customer.ShouldNotBeNull();
        customer.Id.Value.ShouldNotBe(Guid.Empty);
        customer.Name.ShouldBe(_validName);
        customer.Email.ShouldBe(_validEmail);
        customer.PhoneNumber.ShouldBe(_validPhone);
        customer.DateOfBirth.ShouldBe(_validBirthDate);
        customer.Address.ShouldBe(_validAddress);
        customer.DriversLicense.ShouldBe(_validLicense);
        customer.Status.ShouldBe(CustomerStatus.Active);
        customer.RegisteredAtUtc.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(1));
        customer.UpdatedAtUtc.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Register_ShouldRaiseCustomerRegisteredEvent()
    {
        // Act
        var customer = Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            _validBirthDate,
            _validAddress,
            _validLicense);

        // Assert
        var events = customer.GetDomainEvents();
        events.ShouldNotBeEmpty();
        var registeredEvent = events.ShouldHaveSingleItem();
        registeredEvent.ShouldBeOfType<CustomerRegistered>();

        var evt = (CustomerRegistered)registeredEvent;
        evt.CustomerId.ShouldBe(customer.Id);
        evt.Name.ShouldBe(_validName);
        evt.Email.ShouldBe(_validEmail);
        evt.DateOfBirth.ShouldBe(_validBirthDate);
    }

    [Fact]
    public void Register_WithCustomerUnder18_ShouldThrowArgumentException()
    {
        // Arrange - 17 years old
        var tooYoung = BirthDate.Of(DateOnly.FromDateTime(DateTime.Now.AddYears(-17)));

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            tooYoung,
            _validAddress,
            _validLicense));

        ex.Message.ShouldContain("18");
    }

    [Fact]
    public void Register_WithExpiredLicense_ShouldThrowArgumentException()
    {
        // Arrange - License expired yesterday
        var issueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-10));
        var expiryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
        var expiredLicense = DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate);

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            _validBirthDate,
            _validAddress,
            expiredLicense));

        ex.Message.ShouldContain("expired");
    }

    [Fact]
    public void Register_WithLicenseExpiringInLessThan30Days_ShouldThrowArgumentException()
    {
        // Arrange - License expires in 15 days (less than minimum 30 days)
        var issueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
        var expiryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(15));
        var soonExpiringLicense = DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate);

        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            _validBirthDate,
            _validAddress,
            soonExpiringLicense));

        ex.Message.ShouldContain("30 days");
    }

    [Fact]
    public void Register_WithLicenseExpiringInExactly30Days_ShouldSucceed()
    {
        // Arrange - License expires in exactly 30 days (edge case - should be valid)
        var issueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
        var expiryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        var almostExpiringLicense = DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate);

        // Act
        var customer = Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            _validBirthDate,
            _validAddress,
            almostExpiringLicense);

        // Assert
        customer.ShouldNotBeNull();
    }

    [Fact]
    public void FullName_ShouldReturnNameFullName()
    {
        // Arrange
        var customer = Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            _validBirthDate,
            _validAddress,
            _validLicense);

        // Act
        var fullName = customer.FullName;

        // Assert
        fullName.ShouldBe("Max Mustermann");
    }

    [Fact]
    public void FormalName_ShouldReturnNameFormalName()
    {
        // Arrange
        var customer = Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            _validBirthDate,
            _validAddress,
            _validLicense);

        // Act
        var formalName = customer.FormalName;

        // Assert
        formalName.ShouldBe("Herr Max Mustermann");
    }

    [Fact]
    public void Age_ShouldReturnCorrectAge()
    {
        // Arrange - Born in 1990, so 35 years old in 2025
        var customer = Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            _validBirthDate,
            _validAddress,
            _validLicense);

        // Act
        var age = customer.Age;

        // Assert
        age.ShouldBeGreaterThanOrEqualTo(18);
        age.ShouldBeLessThan(150); // Sanity check
    }

    [Fact]
    public void UpdateProfile_WithChangedValues_ShouldReturnNewInstance()
    {
        // Arrange
        var customer = Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            _validBirthDate,
            _validAddress,
            _validLicense);

        var newName = CustomerName.Of("Anna", "Schmidt", Salutation.Frau);
        var newPhone = PhoneNumber.Of("0160 98765432");
        var newAddress = Address.Of("Neue Stra\u00DFe", "456", "München", PostalCode.Of("80331"), City.Of("München"), "Deutschland");

        // Act
        var updatedCustomer = customer.UpdateProfile(newName, newPhone, newAddress);

        // Assert
        updatedCustomer.ShouldNotBeSameAs(customer); // New instance (immutable)
        updatedCustomer.Id.ShouldBe(customer.Id); // Same ID
        updatedCustomer.Name.ShouldBe(newName);
        updatedCustomer.PhoneNumber.ShouldBe(newPhone);
        updatedCustomer.Address.ShouldBe(newAddress);
        updatedCustomer.Email.ShouldBe(customer.Email); // Email unchanged
        updatedCustomer.UpdatedAtUtc.ShouldBeGreaterThan(customer.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateProfile_WithSameValues_ShouldReturnSameInstance()
    {
        // Arrange
        var customer = Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            _validBirthDate,
            _validAddress,
            _validLicense);

        // Act
        var updatedCustomer = customer.UpdateProfile(_validName, _validPhone, _validAddress);

        // Assert
        updatedCustomer.ShouldBeSameAs(customer); // Same instance if no changes
    }

    [Fact]
    public void UpdateProfile_ShouldRaiseCustomerProfileUpdatedEvent()
    {
        // Arrange
        var customer = Customer.Register(
            _validName,
            _validEmail,
            _validPhone,
            _validBirthDate,
            _validAddress,
            _validLicense);

        customer.ClearDomainEvents(); // Clear registration event

        var newName = CustomerName.Of("Anna", "Schmidt");
        var newPhone = PhoneNumber.Of("0160 98765432");
        var newAddress = Address.Of("Neue Stra\u00DFe", "456", "München", PostalCode.Of("80331"), City.Of("München"), "Deutschland");

        // Act
        var updatedCustomer = customer.UpdateProfile(newName, newPhone, newAddress);

        // Assert
        var events = updatedCustomer.GetDomainEvents();
        events.ShouldNotBeEmpty();
        var profileUpdatedEvent = events.ShouldHaveSingleItem();
        profileUpdatedEvent.ShouldBeOfType<CustomerProfileUpdated>();
    }

    [Fact]
    public void MinimumAgeYears_ShouldBe18()
    {
        // Assert
        Customer.MinimumAgeYears.ShouldBe(18);
    }

    [Fact]
    public void MinimumLicenseValidityDays_ShouldBe30()
    {
        // Assert
        Customer.MinimumLicenseValidityDays.ShouldBe(30);
    }
}

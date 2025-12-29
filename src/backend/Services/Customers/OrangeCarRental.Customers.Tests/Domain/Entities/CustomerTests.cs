using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;
using SmartSolutionsLab.OrangeCarRental.Customers.Tests.Builders;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Domain.Entities;

public class CustomerTests
{
    [Fact]
    public void Register_WithValidData_ShouldCreateCustomer()
    {
        // Arrange & Act
        var customer = CustomerBuilder.Default().Build();

        // Assert
        customer.ShouldNotBeNull();
        customer.Id.Value.ShouldNotBe(Guid.Empty);
        customer.Status.ShouldBe(CustomerStatus.Active);
        customer.RegisteredAtUtc.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(1));
        customer.UpdatedAtUtc.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Register_ShouldRaiseCustomerRegisteredEvent()
    {
        // Arrange & Act
        var customer = CustomerBuilder.Default().Build();

        // Assert - DomainEvents contains uncommitted events from AggregateRoot
        var events = customer.DomainEvents;
        events.ShouldNotBeEmpty();
        var registeredEvent = events.ShouldHaveSingleItem();
        registeredEvent.ShouldBeOfType<CustomerRegistered>();
    }

    [Fact]
    public void Register_WithCustomerUnder18_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => CustomerBuilder.Default().UnderAge().Build();

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("18");
    }

    [Fact]
    public void Register_WithExpiredLicense_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => CustomerBuilder.Default().WithExpiredLicense().Build();

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("expired");
    }

    [Fact]
    public void Register_WithLicenseExpiringInLessThan30Days_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => CustomerBuilder.Default().WithLicenseExpiringSoon().Build();

        // Assert
        var ex = Should.Throw<ArgumentException>(act);
        ex.Message.ShouldContain("30 days");
    }

    [Fact]
    public void Register_WithLicenseExpiringInExactly30Days_ShouldSucceed()
    {
        // Arrange & Act
        var customer = CustomerBuilder.Default().WithLicenseExpiringInExactly30Days().Build();

        // Assert
        customer.ShouldNotBeNull();
    }

    [Fact]
    public void FullName_ShouldReturnNameFullName()
    {
        // Arrange
        var customer = CustomerBuilder.MaxMustermann().Build();

        // Act
        var fullName = customer.FullName;

        // Assert
        fullName.ShouldBe("Max Mustermann");
    }

    [Fact]
    public void FormalName_ShouldReturnNameFormalName()
    {
        // Arrange
        var customer = CustomerBuilder.MaxMustermann().Build();

        // Act
        var formalName = customer.FormalName;

        // Assert
        formalName.ShouldBe("Herr Max Mustermann");
    }

    [Fact]
    public void Age_ShouldReturnCorrectAge()
    {
        // Arrange - 35 years old
        var customer = CustomerBuilder.Default()
            .WithAge(35)
            .Build();

        // Act
        var age = customer.Age;

        // Assert
        age.ShouldBe(35);
    }

    [Fact]
    public void UpdateProfile_WithChangedValues_ShouldUpdateState()
    {
        // Arrange
        var customer = CustomerBuilder.MaxMustermann().Build();
        var originalEmail = customer.Email;
        var newName = CustomerName.Of("Anna", "Schmidt", Salutation.Frau);
        var newPhone = PhoneNumber.From("0160 98765432");
        var newAddress = Address.Of("Neue Straße 456", "München", "80331", "Deutschland");

        // Act - immutable pattern returns new instance
        var updatedCustomer = customer.UpdateProfile(newName, newPhone, newAddress);

        // Assert - verify the new instance has updated values
        updatedCustomer.Name.ShouldBe(newName);
        updatedCustomer.PhoneNumber.ShouldBe(newPhone);
        updatedCustomer.Address.ShouldBe(newAddress);
        updatedCustomer.Email.ShouldBe(originalEmail); // Email unchanged
    }

    [Fact]
    public void UpdateProfile_WithSameValues_ShouldReturnSameInstance()
    {
        // Arrange
        var customer = CustomerBuilder.Default().Build();
        var name = customer.Name;
        var phone = customer.PhoneNumber;
        var address = customer.Address;

        // Act - immutable pattern returns same instance if no changes
        var result = customer.UpdateProfile(name, phone, address);

        // Assert - same instance returned when values unchanged
        result.ShouldBeSameAs(customer);
    }

    [Fact]
    public void UpdateProfile_ShouldRaiseCustomerProfileUpdatedEvent()
    {
        // Arrange
        var customer = CustomerBuilder.Default().Build();

        var newName = CustomerName.Of("Anna", "Schmidt");
        var newPhone = PhoneNumber.From("0160 98765432");
        var newAddress = Address.Of("Neue Straße 456", "München", "80331", "Deutschland");

        // Act - immutable pattern returns new instance with the event
        var updatedCustomer = customer.UpdateProfile(newName, newPhone, newAddress);

        // Assert - the updated instance should have the event
        updatedCustomer.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<CustomerProfileUpdated>();
    }

    [Fact]
    public void MinimumRegistrationAgeYears_ShouldBe18()
    {
        // Assert
        Customer.MinimumRegistrationAgeYears.ShouldBe(18);
    }

    [Fact]
    public void MinimumRentalAgeYears_ShouldBe21()
    {
        // Assert
        Customer.MinimumRentalAgeYears.ShouldBe(21);
    }

    [Fact]
    public void MinimumLicenseHeldYears_ShouldBe1()
    {
        // Assert
        Customer.MinimumLicenseHeldYears.ShouldBe(1);
    }

    [Fact]
    public void MinimumLicenseValidityDays_ShouldBe30()
    {
        // Assert
        Customer.MinimumLicenseValidityDays.ShouldBe(30);
    }

    #region Named Customer Tests

    [Fact]
    public void MaxMustermann_CreatesGermanMaleCustomer()
    {
        // Arrange & Act
        var customer = CustomerBuilder.MaxMustermann().Build();

        // Assert - direct property access (non-nullable value objects)
        customer.Name.FirstName.Value.ShouldBe("Max");
        customer.Name.LastName.Value.ShouldBe("Mustermann");
        customer.Email.Value.ShouldBe("max.mustermann@example.de");
        customer.Address.City.Value.ShouldBe("Berlin");
    }

    [Fact]
    public void AnnaSchmidt_CreatesGermanFemaleCustomer()
    {
        // Arrange & Act
        var customer = CustomerBuilder.AnnaSchmidt().Build();

        // Assert - direct property access (non-nullable value objects)
        customer.Name.FirstName.Value.ShouldBe("Anna");
        customer.Name.LastName.Value.ShouldBe("Schmidt");
        customer.Email.Value.ShouldBe("anna.schmidt@example.com");
        customer.Address.City.Value.ShouldBe("München");
    }

    [Fact]
    public void YoungCustomer_CreatesCustomerAtMinimumAge()
    {
        // Arrange & Act
        var customer = CustomerBuilder.YoungCustomer().Build();

        // Assert
        customer.Age.ShouldBe(18);
    }

    [Fact]
    public void SeniorCustomer_CreatesSeniorCustomer()
    {
        // Arrange & Act
        var customer = CustomerBuilder.SeniorCustomer().Build();

        // Assert
        customer.Age.ShouldBeGreaterThanOrEqualTo(65);
    }

    #endregion
}

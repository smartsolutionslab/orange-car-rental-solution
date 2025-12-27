using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.CommandServices;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.EventSourcing;
using SmartSolutionsLab.OrangeCarRental.Customers.Tests.Builders;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Infrastructure.CommandServices;

public class CustomerCommandServiceTests
{
    private readonly Mock<IEventSourcedCustomerRepository> _repositoryMock = new();
    private readonly CustomerCommandService _service;

    public CustomerCommandServiceTests()
    {
        _service = new CustomerCommandService(_repositoryMock.Object);
    }

    #region RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldCreateAndSaveCustomer()
    {
        // Arrange
        var name = CustomerName.Of("Max", "Mustermann", Salutation.Herr);
        var email = Email.From("max.mustermann@example.com");
        var phone = PhoneNumber.From("0151 12345678");
        var birthDate = BirthDate.Of(new DateOnly(1990, 1, 1));
        var address = Address.Of("Hauptstraße 123", "Berlin", "10115", "Deutschland");
        var issueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5));
        var expiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(5));
        var license = DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate);

        // Act
        var result = await _service.RegisterAsync(name, email, phone, birthDate, address, license);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(name);
        result.Email.ShouldBe(email);
        result.PhoneNumber.ShouldBe(phone);
        result.DateOfBirth.ShouldBe(birthDate);
        result.Address.ShouldBe(address);
        result.DriversLicense.ShouldBe(license);
        result.Status.ShouldBe(CustomerStatus.Active);

        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCallSaveAsyncWithNewCustomer()
    {
        // Arrange
        var name = CustomerName.Of("Anna", "Schmidt", Salutation.Frau);
        var email = Email.From("anna.schmidt@example.com");
        var phone = PhoneNumber.From("0160 98765432");
        var birthDate = BirthDate.Of(new DateOnly(1995, 6, 15));
        var address = Address.Of("Neue Straße 456", "München", "80331", "Deutschland");
        var issueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3));
        var expiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(7));
        var license = DriversLicense.Of("DE987654321", "Deutschland", issueDate, expiryDate);

        Customer? savedCustomer = null;
        _repositoryMock
            .Setup(r => r.SaveAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>((c, _) => savedCustomer = c)
            .Returns(Task.CompletedTask);

        // Act
        await _service.RegisterAsync(name, email, phone, birthDate, address, license);

        // Assert
        savedCustomer.ShouldNotBeNull();
        savedCustomer.Name.ShouldBe(name);
        savedCustomer.Email.ShouldBe(email);
    }

    #endregion

    #region UpdateProfileAsync Tests

    [Fact]
    public async Task UpdateProfileAsync_WithExistingCustomer_ShouldUpdateAndSave()
    {
        // Arrange
        var existingCustomer = CustomerBuilder.MaxMustermann().Build();
        var customerId = existingCustomer.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        var newName = CustomerName.Of("Max", "Schmidt", Salutation.Herr);
        var newPhone = PhoneNumber.From("0170 11111111");
        var newAddress = Address.Of("Neue Straße 789", "Hamburg", "20095", "Deutschland");

        // Act
        var result = await _service.UpdateProfileAsync(customerId, newName, newPhone, newAddress);

        // Assert
        result.Name.ShouldBe(newName);
        result.PhoneNumber.ShouldBe(newPhone);
        result.Address.ShouldBe(newAddress);

        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_WithNonExistingCustomer_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var customerId = CustomerIdentifier.New();
        var emptyCustomer = new Customer(); // Not registered, HasBeenCreated = false

        _repositoryMock
            .Setup(r => r.LoadAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyCustomer);

        var name = CustomerName.Of("Test", "Test");
        var phone = PhoneNumber.From("0151 12345678");
        var address = Address.Of("Test", "Test", "12345", "Deutschland");

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(
            () => _service.UpdateProfileAsync(customerId, name, phone, address));

        ex.Message.ShouldContain(customerId.Value.ToString());
        ex.Message.ShouldContain("not found");
    }

    #endregion

    #region UpdateDriversLicenseAsync Tests

    [Fact]
    public async Task UpdateDriversLicenseAsync_WithExistingCustomer_ShouldUpdateAndSave()
    {
        // Arrange
        var existingCustomer = CustomerBuilder.MaxMustermann().Build();
        var customerId = existingCustomer.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        var newIssueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1));
        var newExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(9));
        var newLicense = DriversLicense.Of("NEWLICENSE123", "Deutschland", newIssueDate, newExpiryDate);

        // Act
        var result = await _service.UpdateDriversLicenseAsync(customerId, newLicense);

        // Assert
        result.DriversLicense.ShouldBe(newLicense);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateDriversLicenseAsync_WithNonExistingCustomer_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var customerId = CustomerIdentifier.New();
        var emptyCustomer = new Customer();

        _repositoryMock
            .Setup(r => r.LoadAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyCustomer);

        var issueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1));
        var expiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(9));
        var license = DriversLicense.Of("TEST123", "Deutschland", issueDate, expiryDate);

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(
            () => _service.UpdateDriversLicenseAsync(customerId, license));

        ex.Message.ShouldContain("not found");
    }

    #endregion

    #region ChangeStatusAsync Tests

    [Fact]
    public async Task ChangeStatusAsync_WithExistingCustomer_ShouldChangeStatusAndSave()
    {
        // Arrange
        var existingCustomer = CustomerBuilder.MaxMustermann().Build();
        var customerId = existingCustomer.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        // Act
        var result = await _service.ChangeStatusAsync(customerId, CustomerStatus.Suspended, "Payment issues");

        // Assert
        result.Status.ShouldBe(CustomerStatus.Suspended);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangeStatusAsync_ToBlocked_ShouldWork()
    {
        // Arrange
        var existingCustomer = CustomerBuilder.MaxMustermann().Build();
        var customerId = existingCustomer.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        // Act
        var result = await _service.ChangeStatusAsync(customerId, CustomerStatus.Blocked, "Fraud detected");

        // Assert
        result.Status.ShouldBe(CustomerStatus.Blocked);
    }

    [Fact]
    public async Task ChangeStatusAsync_WithNonExistingCustomer_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var customerId = CustomerIdentifier.New();
        var emptyCustomer = new Customer();

        _repositoryMock
            .Setup(r => r.LoadAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyCustomer);

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(
            () => _service.ChangeStatusAsync(customerId, CustomerStatus.Suspended, "Test"));

        ex.Message.ShouldContain("not found");
    }

    #endregion

    #region UpdateEmailAsync Tests

    [Fact]
    public async Task UpdateEmailAsync_WithExistingCustomer_ShouldUpdateAndSave()
    {
        // Arrange
        var existingCustomer = CustomerBuilder.MaxMustermann().Build();
        var customerId = existingCustomer.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        var newEmail = Email.From("max.new.email@example.com");

        // Act
        var result = await _service.UpdateEmailAsync(customerId, newEmail);

        // Assert
        result.Email.ShouldBe(newEmail);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEmailAsync_WithNonExistingCustomer_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var customerId = CustomerIdentifier.New();
        var emptyCustomer = new Customer();

        _repositoryMock
            .Setup(r => r.LoadAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyCustomer);

        var newEmail = Email.From("new@example.com");

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(
            () => _service.UpdateEmailAsync(customerId, newEmail));

        ex.Message.ShouldContain("not found");
    }

    #endregion

    #region CancellationToken Tests

    [Fact]
    public async Task RegisterAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var name = CustomerName.Of("Test", "User");
        var email = Email.From("test@example.com");
        var phone = PhoneNumber.From("0151 12345678");
        var birthDate = BirthDate.Of(new DateOnly(1990, 1, 1));
        var address = Address.Of("Test", "Test", "12345", "Deutschland");
        var issueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5));
        var expiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(5));
        var license = DriversLicense.Of("TEST123", "Deutschland", issueDate, expiryDate);

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Act
        await _service.RegisterAsync(name, email, phone, birthDate, address, license, token);

        // Assert
        _repositoryMock.Verify(
            r => r.SaveAsync(It.IsAny<Customer>(), token),
            Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var existingCustomer = CustomerBuilder.MaxMustermann().Build();
        var customerId = existingCustomer.Id;

        _repositoryMock
            .Setup(r => r.LoadAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var name = CustomerName.Of("Updated", "Name");
        var phone = PhoneNumber.From("0170 99999999");
        var address = Address.Of("New", "City", "00000", "Deutschland");

        // Act
        await _service.UpdateProfileAsync(customerId, name, phone, address, token);

        // Assert
        _repositoryMock.Verify(r => r.LoadAsync(customerId, token), Times.Once);
        _repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Customer>(), token), Times.Once);
    }

    #endregion
}

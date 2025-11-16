using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Application.Commands;

public class RegisterCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> customerRepositoryMock = new();
    private readonly RegisterCustomerCommandHandler handler;

    public RegisterCustomerCommandHandlerTests()
    {
        handler = new RegisterCustomerCommandHandler(customerRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldRegisterCustomer()
    {
        // Arrange
        var command = CreateValidCommand();

        customerRepositoryMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Customer? addedCustomer = null;
        customerRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>((customer, _) => addedCustomer = customer)
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.CustomerIdentifier.ShouldNotBe(Guid.Empty);
        result.Email.ShouldBe(command.Email.Value.ToLowerInvariant());
        result.Status.ShouldBe("Customer registered successfully");
        result.RegisteredAtUtc.ShouldBeInRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(1));

        addedCustomer.ShouldNotBeNull();
        addedCustomer.Email.ShouldBe(command.Email);
        addedCustomer.Name.ShouldBe(command.Name);
        addedCustomer.PhoneNumber.ShouldBe(command.PhoneNumber);

        customerRepositoryMock.Verify(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
        customerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = CreateValidCommand();

        customerRepositoryMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Email already exists

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            handler.HandleAsync(command, CancellationToken.None));

        ex.Message.ShouldContain(command.Email.Value);
        ex.Message.ShouldContain("already exists");

        customerRepositoryMock.Verify(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
        customerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithCustomerUnder18_ShouldThrowArgumentException()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            DateOfBirth = BirthDate.Of(DateOnly.FromDateTime(DateTime.Now.AddYears(-17))) // 17 years old
        };

        customerRepositoryMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() =>
            handler.HandleAsync(command, CancellationToken.None));

        customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithExpiredLicense_ShouldThrowArgumentException()
    {
        // Arrange
        var issueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-10));
        var expiryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)); // Expired yesterday

        var command = CreateValidCommand() with
        {
            DriversLicense = DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate)
        };

        customerRepositoryMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() =>
            handler.HandleAsync(command, CancellationToken.None));

        customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallSaveChanges()
    {
        // Arrange
        var command = CreateValidCommand();

        customerRepositoryMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        customerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var command = CreateValidCommand();
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        customerRepositoryMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            handler.HandleAsync(command, cts.Token));
    }

    private static RegisterCustomerCommand CreateValidCommand()
    {
        var issueDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
        var expiryDate = DateOnly.FromDateTime(DateTime.Now.AddYears(5));

        return new RegisterCustomerCommand(
            CustomerName.Of("Max", "Mustermann", Salutation.Herr),
            Email.Of("max.mustermann@example.com"),
            PhoneNumber.Of("0151 12345678"),
            BirthDate.Of(new DateOnly(1990, 1, 1)),
            Address.Of("Hauptstraße 123", "Berlin", "10115", "Deutschland"),
            DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate));
    }
}

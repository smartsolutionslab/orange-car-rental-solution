using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Application.Commands;

public class RegisterCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerCommandService> _commandServiceMock = new();
    private readonly Mock<ICustomersUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly RegisterCustomerCommandHandler handler;

    public RegisterCustomerCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.Customers).Returns(_customerRepositoryMock.Object);
        handler = new RegisterCustomerCommandHandler(_commandServiceMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldRegisterCustomer()
    {
        // Arrange
        var command = CreateValidCommand();

        _customerRepositoryMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var registeredCustomer = CreateCustomerFromCommand(command);
        _commandServiceMock
            .Setup(x => x.RegisterAsync(
                command.Name,
                command.Email,
                command.PhoneNumber,
                command.DateOfBirth,
                command.Address,
                command.DriversLicense,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(registeredCustomer);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.CustomerIdentifier.ShouldNotBe(Guid.Empty);
        result.Email.ShouldBe(command.Email.Value.ToLowerInvariant());
        result.Status.ShouldBe("Customer registered successfully");

        _customerRepositoryMock.Verify(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _commandServiceMock.Verify(x => x.RegisterAsync(
            command.Name,
            command.Email,
            command.PhoneNumber,
            command.DateOfBirth,
            command.Address,
            command.DriversLicense,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = CreateValidCommand();

        _customerRepositoryMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Email already exists

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            handler.HandleAsync(command, CancellationToken.None));

        ex.Message.ShouldContain(command.Email.Value);
        ex.Message.ShouldContain("already exists");

        _customerRepositoryMock.Verify(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _commandServiceMock.Verify(x => x.RegisterAsync(
            It.IsAny<CustomerName>(),
            It.IsAny<Email>(),
            It.IsAny<PhoneNumber>(),
            It.IsAny<BirthDate>(),
            It.IsAny<Address>(),
            It.IsAny<DriversLicense>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithCustomerUnder18_ShouldThrowArgumentException()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            DateOfBirth = BirthDate.Of(DateOnly.FromDateTime(DateTime.Now.AddYears(-17))) // 17 years old
        };

        _customerRepositoryMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _commandServiceMock
            .Setup(x => x.RegisterAsync(
                It.IsAny<CustomerName>(),
                It.IsAny<Email>(),
                It.IsAny<PhoneNumber>(),
                It.IsAny<BirthDate>(),
                It.IsAny<Address>(),
                It.IsAny<DriversLicense>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Customer must be at least 18 years old"));

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() =>
            handler.HandleAsync(command, CancellationToken.None));
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

        _customerRepositoryMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _commandServiceMock
            .Setup(x => x.RegisterAsync(
                It.IsAny<CustomerName>(),
                It.IsAny<Email>(),
                It.IsAny<PhoneNumber>(),
                It.IsAny<BirthDate>(),
                It.IsAny<Address>(),
                It.IsAny<DriversLicense>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Driver's license is expired"));

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() =>
            handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ShouldCallCommandService()
    {
        // Arrange
        var command = CreateValidCommand();

        _customerRepositoryMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var registeredCustomer = CreateCustomerFromCommand(command);
        _commandServiceMock
            .Setup(x => x.RegisterAsync(
                It.IsAny<CustomerName>(),
                It.IsAny<Email>(),
                It.IsAny<PhoneNumber>(),
                It.IsAny<BirthDate>(),
                It.IsAny<Address>(),
                It.IsAny<DriversLicense>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(registeredCustomer);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _commandServiceMock.Verify(x => x.RegisterAsync(
            command.Name,
            command.Email,
            command.PhoneNumber,
            command.DateOfBirth,
            command.Address,
            command.DriversLicense,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var command = CreateValidCommand();
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        _customerRepositoryMock
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
            Email.From("max.mustermann@example.com"),
            PhoneNumber.From("0151 12345678"),
            BirthDate.Of(new DateOnly(1990, 1, 1)),
            Address.Of("Hauptstraße 123", "Berlin", "10115", "Deutschland"),
            DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate));
    }

    private static Customer CreateCustomerFromCommand(RegisterCustomerCommand command)
    {
        var customer = new Customer();
        customer.Register(
            command.Name,
            command.Email,
            command.PhoneNumber,
            command.DateOfBirth,
            command.Address,
            command.DriversLicense);
        return customer;
    }
}

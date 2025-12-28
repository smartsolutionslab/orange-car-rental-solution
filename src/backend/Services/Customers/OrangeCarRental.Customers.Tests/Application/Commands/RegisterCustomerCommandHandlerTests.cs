using Moq;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Tests.Application.Commands;

public class RegisterCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock = new();
    private readonly Mock<ICustomersUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICustomerRepository> _unitOfWorkCustomersMock = new();
    private readonly RegisterCustomerCommandHandler _handler;

    public RegisterCustomerCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.Customers).Returns(_unitOfWorkCustomersMock.Object);
        _handler = new RegisterCustomerCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldRegisterCustomer()
    {
        // Arrange
        var command = CreateValidCommand();

        _unitOfWorkCustomersMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.CustomerIdentifier.ShouldNotBe(Guid.Empty);
        result.Email.ShouldBe(command.Email.Value.ToLowerInvariant());
        result.Status.ShouldBe("Customer registered successfully");

        _unitOfWorkCustomersMock.Verify(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = CreateValidCommand();

        _unitOfWorkCustomersMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Email already exists

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            _handler.HandleAsync(command, CancellationToken.None));

        ex.Message.ShouldContain(command.Email.Value);
        ex.Message.ShouldContain("already exists");

        _unitOfWorkCustomersMock.Verify(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateCustomerAndPersist()
    {
        // Arrange
        var command = CreateValidCommand();
        Customer? capturedCustomer = null;

        _unitOfWorkCustomersMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>((c, _) => capturedCustomer = c)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedCustomer.ShouldNotBeNull();
        capturedCustomer.Email.ShouldBe(command.Email);
        capturedCustomer.Name.ShouldBe(command.Name);
        capturedCustomer.PhoneNumber.ShouldBe(command.PhoneNumber);
    }

    [Fact]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var command = CreateValidCommand();
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        _unitOfWorkCustomersMock
            .Setup(x => x.ExistsWithEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _handler.HandleAsync(command, cts.Token));
    }

    private static RegisterCustomerCommand CreateValidCommand()
    {
        var issueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5));
        var expiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(5));

        return new RegisterCustomerCommand(
            CustomerName.Of("Max", "Mustermann", Salutation.Herr),
            Email.From("max.mustermann@example.com"),
            PhoneNumber.From("0151 12345678"),
            BirthDate.Of(new DateOnly(1990, 1, 1)),
            Address.Of("Hauptstraße 123", "Berlin", "10115", "Deutschland"),
            DriversLicense.Of("DE123456789", "Deutschland", issueDate, expiryDate));
    }
}

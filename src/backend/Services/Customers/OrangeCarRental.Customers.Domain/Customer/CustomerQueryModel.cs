using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.EventSourcing;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Query model (state) for the Customer aggregate.
///     Built from domain events and used for both:
///     - Internal aggregate state during command processing
///     - Read model for queries
/// </summary>
public sealed record CustomerQueryModel : QueryModel<CustomerQueryModel, CustomerIdentifier>
{
    public override CustomerIdentifier Id { get; init; } = CustomerIdentifier.Empty;
    public CustomerName? Name { get; init; }
    public Email? Email { get; init; }
    public PhoneNumber? PhoneNumber { get; init; }
    public BirthDate? DateOfBirth { get; init; }
    public Address? Address { get; init; }
    public DriversLicense? DriversLicense { get; init; }
    public CustomerStatus Status { get; init; } = CustomerStatus.Active;
    public DateTime RegisteredAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }

    /// <summary>
    ///     Indicates whether this customer has been created (registered).
    /// </summary>
    public override bool HasBeenCreated => Id != CustomerIdentifier.Empty;

    /// <summary>
    ///     Gets the customer's full name, or empty string if not set.
    /// </summary>
    public string FullName => Name?.FullName ?? string.Empty;

    /// <summary>
    ///     Gets the customer's formal name with salutation, or empty string if not set.
    /// </summary>
    public string FormalName => Name?.FormalName ?? string.Empty;

    /// <summary>
    ///     Gets the customer's current age in years.
    /// </summary>
    public int Age => DateOfBirth?.Age ?? 0;

    public CustomerQueryModel()
    {
        On<CustomerRegistered>(Handle);
        On<CustomerProfileUpdated>(Handle);
        On<CustomerStatusChanged>(Handle);
        On<DriversLicenseUpdated>(Handle);
        On<CustomerEmailChanged>(Handle);
    }

    private static CustomerQueryModel Handle(CustomerQueryModel state, CustomerRegistered @event) =>
        state with
        {
            Id = @event.CustomerIdentifier,
            Name = @event.Name,
            Email = @event.Email,
            PhoneNumber = @event.PhoneNumber,
            DateOfBirth = @event.DateOfBirth,
            Address = @event.Address,
            DriversLicense = @event.DriversLicense,
            Status = CustomerStatus.Active,
            RegisteredAtUtc = @event.RegisteredAtUtc,
            UpdatedAtUtc = @event.RegisteredAtUtc
        };

    private static CustomerQueryModel Handle(CustomerQueryModel state, CustomerProfileUpdated @event) =>
        state with
        {
            Name = @event.Name,
            PhoneNumber = @event.PhoneNumber,
            Address = @event.Address,
            UpdatedAtUtc = @event.UpdatedAtUtc
        };

    private static CustomerQueryModel Handle(CustomerQueryModel state, CustomerStatusChanged @event) =>
        state with
        {
            Status = @event.NewStatus,
            UpdatedAtUtc = @event.ChangedAtUtc
        };

    private static CustomerQueryModel Handle(CustomerQueryModel state, DriversLicenseUpdated @event) =>
        state with
        {
            DriversLicense = @event.NewLicense,
            UpdatedAtUtc = @event.UpdatedAtUtc
        };

    private static CustomerQueryModel Handle(CustomerQueryModel state, CustomerEmailChanged @event) =>
        state with
        {
            Email = @event.NewEmail,
            UpdatedAtUtc = @event.ChangedAtUtc
        };
}

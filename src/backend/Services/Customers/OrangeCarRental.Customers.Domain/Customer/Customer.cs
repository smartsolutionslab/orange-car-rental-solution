using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Customer aggregate root.
///     Represents a customer in the Orange Car Rental system with German market-specific validation.
///     Enforces business rules such as minimum age (18+), valid driver's license, and GDPR compliance.
/// </summary>
public sealed class Customer : AggregateRoot<CustomerIdentifier>
{
    /// <summary>
    ///     Minimum age required to rent a vehicle in Germany.
    /// </summary>
    public const int MinimumAgeYears = 18;

    /// <summary>
    ///     Minimum days before license expiry to allow rentals.
    ///     Prevents customers from renting with a license about to expire.
    /// </summary>
    public const int MinimumLicenseValidityDays = 30;

    // For EF Core - properties will be set by EF Core during materialization
    private Customer()
    {
        Name = default!;
        Email = default!;
        PhoneNumber = default!;
        Address = default!;
        DriversLicense = default!;
    }

    private Customer(
        CustomerIdentifier id,
        CustomerName name,
        Email email,
        PhoneNumber phoneNumber,
        BirthDate dateOfBirth,
        Address address,
        DriversLicense driversLicense)
        : base(id)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Address = address;
        DriversLicense = driversLicense;
        Status = CustomerStatus.Active;
        RegisteredAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new CustomerRegistered(
            Id,
            Name,
            Email,
            DateOfBirth,
            RegisteredAtUtc));
    }

    // IMMUTABLE: Properties can only be set during construction. Methods return new instances.
    public CustomerName Name { get; init; }
    public Email Email { get; init; }
    public PhoneNumber PhoneNumber { get; init; }
    public BirthDate DateOfBirth { get; init; }
    public Address Address { get; init; }
    public DriversLicense DriversLicense { get; init; }
    public CustomerStatus Status { get; init; }
    public DateTime RegisteredAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }

    /// <summary>
    ///     Gets the customer's full name.
    /// </summary>
    public string FullName => Name.FullName;

    /// <summary>
    ///     Gets the customer's formal name with salutation.
    /// </summary>
    public string FormalName => Name.FormalName;

    /// <summary>
    ///     Gets the customer's current age in years.
    /// </summary>
    public int Age => DateOfBirth.Age;

    /// <summary>
    ///     Registers a new customer with German market validation.
    /// </summary>
    /// <param name="name">Customer's name (first name, last name, optional salutation).</param>
    /// <param name="email">Customer's email address.</param>
    /// <param name="phoneNumber">Customer's phone number (German format).</param>
    /// <param name="dateOfBirth">Customer's date of birth.</param>
    /// <param name="address">Customer's address.</param>
    /// <param name="driversLicense">Customer's driver's license.</param>
    /// <returns>A new Customer instance.</returns>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public static Customer Register(
        CustomerName name,
        Email email,
        PhoneNumber phoneNumber,
        BirthDate dateOfBirth,
        Address address,
        DriversLicense driversLicense)
    {
        // Validate age - must be 18+ to rent in Germany
        ValidateAge(dateOfBirth.Value);

        // Validate driver's license
        ValidateDriversLicense(driversLicense, dateOfBirth.Value);

        return new Customer(
            CustomerIdentifier.New(),
            name,
            email,
            phoneNumber,
            dateOfBirth,
            address,
            driversLicense);
    }

    /// <summary>
    ///     Creates a copy of this instance with modified values (used internally for immutable updates).
    ///     Does not raise domain events - caller is responsible for that.
    /// </summary>
    private Customer CreateMutatedCopy(
        CustomerName? name = null,
        Email? email = null,
        PhoneNumber? phoneNumber = null,
        BirthDate? dateOfBirth = null,
        Address? address = null,
        DriversLicense? driversLicense = null,
        CustomerStatus? status = null,
        DateTime? registeredAtUtc = null,
        DateTime? updatedAtUtc = null)
    {
        return new Customer
        {
            Id = Id,
            Name = name ?? Name,
            Email = email ?? Email,
            PhoneNumber = phoneNumber ?? PhoneNumber,
            DateOfBirth = dateOfBirth ?? DateOfBirth,
            Address = address ?? Address,
            DriversLicense = driversLicense ?? DriversLicense,
            Status = status ?? Status,
            RegisteredAtUtc = registeredAtUtc ?? RegisteredAtUtc,
            UpdatedAtUtc = updatedAtUtc ?? UpdatedAtUtc
        };
    }

    /// <summary>
    ///     Updates the customer's profile information.
    ///     Returns a new instance with the updated profile (immutable pattern).
    /// </summary>
    public Customer UpdateProfile(
        CustomerName name,
        PhoneNumber phoneNumber,
        Address address)
    {
        var hasChanges = Name.FirstName.Value != name.FirstName.Value ||
                         Name.LastName.Value != name.LastName.Value ||
                         Name.Salutation != name.Salutation ||
                         PhoneNumber != phoneNumber ||
                         Address != address;

        if (!hasChanges)
            return this;

        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            name,
            phoneNumber: phoneNumber,
            address: address,
            updatedAtUtc: now);

        updated.AddDomainEvent(new CustomerProfileUpdated(
            Id,
            name,
            phoneNumber,
            address,
            now));

        return updated;
    }

    /// <summary>
    ///     Updates the customer's driver's license.
    ///     Returns a new instance with the updated license (immutable pattern).
    /// </summary>
    public Customer UpdateDriversLicense(DriversLicense newLicense)
    {
        ValidateDriversLicense(newLicense, DateOfBirth);

        if (DriversLicense == newLicense)
            return this;

        var oldLicense = DriversLicense;
        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            driversLicense: newLicense,
            updatedAtUtc: now);

        updated.AddDomainEvent(new DriversLicenseUpdated(
            Id,
            oldLicense,
            newLicense,
            now));

        return updated;
    }

    /// <summary>
    ///     Changes the customer's account status.
    ///     Returns a new instance with the updated status (immutable pattern).
    /// </summary>
    public Customer ChangeStatus(CustomerStatus newStatus, string reason)
    {
        Ensure.That(reason, nameof(reason))
            .IsNotNullOrWhiteSpace();

        if (Status == newStatus)
            return this;

        var oldStatus = Status;
        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            status: newStatus,
            updatedAtUtc: now);

        updated.AddDomainEvent(new CustomerStatusChanged(
            Id,
            oldStatus,
            newStatus,
            reason,
            now));

        return updated;
    }

    /// <summary>
    ///     Updates the customer's email address.
    ///     Returns a new instance with the updated email (immutable pattern).
    /// </summary>
    public Customer UpdateEmail(Email newEmail)
    {
        if (Email == newEmail)
            return this;

        var oldEmail = Email;
        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            email: newEmail,
            updatedAtUtc: now);

        updated.AddDomainEvent(new CustomerEmailChanged(
            Id,
            oldEmail,
            newEmail,
            now));

        return updated;
    }

    /// <summary>
    ///     Activates the customer account.
    ///     Returns a new instance with active status (immutable pattern).
    /// </summary>
    public Customer Activate(string reason = "Account activated")
    {
        if (Status == CustomerStatus.Active)
            return this;

        return ChangeStatus(CustomerStatus.Active, reason);
    }

    /// <summary>
    ///     Suspends the customer account (temporary).
    ///     Returns a new instance with suspended status (immutable pattern).
    /// </summary>
    public Customer Suspend(string reason)
    {
        Ensure.That(reason, nameof(reason))
            .IsNotNullOrWhiteSpace();

        if (Status == CustomerStatus.Suspended)
            return this;

        return ChangeStatus(CustomerStatus.Suspended, reason);
    }

    /// <summary>
    ///     Blocks the customer account (more severe than suspension).
    ///     Returns a new instance with blocked status (immutable pattern).
    /// </summary>
    public Customer Block(string reason)
    {
        Ensure.That(reason, nameof(reason))
            .IsNotNullOrWhiteSpace();

        if (Status == CustomerStatus.Blocked)
            return this;

        return ChangeStatus(CustomerStatus.Blocked, reason);
    }

    /// <summary>
    ///     Checks if the customer can make reservations.
    /// </summary>
    public bool CanMakeReservation()
    {
        // Must be active
        if (Status != CustomerStatus.Active)
            return false;

        // License must be valid
        if (!DriversLicense.IsValid())
            return false;

        // License must have sufficient validity remaining
        if (DriversLicense.DaysUntilExpiry() < MinimumLicenseValidityDays)
            return false;

        return true;
    }

    /// <summary>
    ///     Checks if the customer can make a reservation for a specific rental period.
    /// </summary>
    public bool CanMakeReservationFor(DateOnly startDate, DateOnly endDate)
    {
        if (!CanMakeReservation())
            return false;

        // License must be valid for the entire rental period
        if (!DriversLicense.IsValidOn(startDate) || !DriversLicense.IsValidOn(endDate))
            return false;

        return true;
    }

    /// <summary>
    ///     Validates that the customer meets the minimum age requirement.
    /// </summary>
    private static void ValidateAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;

        // Adjust if birthday hasn't occurred this year
        if (dateOfBirth.AddYears(age) > today)
            age--;

        Ensure.That(dateOfBirth, nameof(dateOfBirth))
            .ThrowIf(age < MinimumAgeYears, $"Customer must be at least {MinimumAgeYears} years old. Current age: {age}");

        // Sanity check - no one over 150 years old
        Ensure.That(dateOfBirth, nameof(dateOfBirth))
            .ThrowIf(age > 150, "Invalid date of birth - age cannot exceed 150 years");

        // Date of birth cannot be in the future
        Ensure.That(dateOfBirth, nameof(dateOfBirth))
            .ThrowIf(dateOfBirth > today, "Date of birth cannot be in the future");
    }

    /// <summary>
    ///     Validates the driver's license against business rules.
    /// </summary>
    private static void ValidateDriversLicense(DriversLicense license, DateOnly dateOfBirth)
    {
        // License must be currently valid
        Ensure.That(license, nameof(license))
            .ThrowIf(!license.IsValid(), $"Driver's license is expired. Expiry date: {license.ExpiryDate}");

        // License must have minimum validity remaining
        Ensure.That(license, nameof(license))
            .ThrowIf(license.DaysUntilExpiry() < MinimumLicenseValidityDays,
                $"Driver's license must be valid for at least {MinimumLicenseValidityDays} days. Days remaining: {license.DaysUntilExpiry()}");

        // License issue date must be after 18th birthday (with some tolerance)
        var minimumLicenseIssueDate = dateOfBirth.AddYears(MinimumAgeYears - 1);
        Ensure.That(license, nameof(license))
            .ThrowIf(license.IssueDate < minimumLicenseIssueDate,
                $"Driver's license issue date seems inconsistent with date of birth. License issued: {license.IssueDate}, Date of birth: {dateOfBirth}");
    }
}

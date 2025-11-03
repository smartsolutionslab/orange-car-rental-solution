using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
/// Customer aggregate root.
/// Represents a customer in the Orange Car Rental system with German market-specific validation.
/// Enforces business rules such as minimum age (18+), valid driver's license, and GDPR compliance.
/// </summary>
public sealed class Customer : AggregateRoot<CustomerId>
{
    /// <summary>
    /// Minimum age required to rent a vehicle in Germany.
    /// </summary>
    public const int MinimumAgeYears = 18;

    /// <summary>
    /// Minimum days before license expiry to allow rentals.
    /// Prevents customers from renting with a license about to expire.
    /// </summary>
    public const int MinimumLicenseValidityDays = 30;

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public Address Address { get; private set; }
    public DriversLicense DriversLicense { get; private set; }
    public CustomerStatus Status { get; private set; }
    public DateTime RegisteredAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    // For EF Core - properties will be set by EF Core during materialization
    private Customer()
    {
        FirstName = default!;
        LastName = default!;
        Email = default!;
        PhoneNumber = default!;
        Address = default!;
        DriversLicense = default!;
    }

    private Customer(
        CustomerId id,
        string firstName,
        string lastName,
        Email email,
        PhoneNumber phoneNumber,
        DateOnly dateOfBirth,
        Address address,
        DriversLicense driversLicense)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
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
            FirstName,
            LastName,
            Email,
            DateOfBirth,
            RegisteredAtUtc));
    }

    /// <summary>
    /// Registers a new customer with German market validation.
    /// </summary>
    /// <param name="firstName">Customer's first name.</param>
    /// <param name="lastName">Customer's last name.</param>
    /// <param name="email">Customer's email address.</param>
    /// <param name="phoneNumber">Customer's phone number (German format).</param>
    /// <param name="dateOfBirth">Customer's date of birth.</param>
    /// <param name="address">Customer's address.</param>
    /// <param name="driversLicense">Customer's driver's license.</param>
    /// <returns>A new Customer instance.</returns>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public static Customer Register(
        string firstName,
        string lastName,
        Email email,
        PhoneNumber phoneNumber,
        DateOnly dateOfBirth,
        Address address,
        DriversLicense driversLicense)
    {
        // Validate names
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));

        var normalizedFirstName = firstName.Trim();
        var normalizedLastName = lastName.Trim();

        if (normalizedFirstName.Length > 100)
            throw new ArgumentException("First name is too long (max 100 characters)", nameof(firstName));

        if (normalizedLastName.Length > 100)
            throw new ArgumentException("Last name is too long (max 100 characters)", nameof(lastName));

        // Validate age - must be 18+ to rent in Germany
        ValidateAge(dateOfBirth);

        // Validate driver's license
        ValidateDriversLicense(driversLicense, dateOfBirth);

        return new Customer(
            CustomerId.New(),
            normalizedFirstName,
            normalizedLastName,
            email,
            phoneNumber,
            dateOfBirth,
            address,
            driversLicense);
    }

    /// <summary>
    /// Updates the customer's profile information.
    /// </summary>
    public void UpdateProfile(
        string firstName,
        string lastName,
        PhoneNumber phoneNumber,
        Address address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));

        var normalizedFirstName = firstName.Trim();
        var normalizedLastName = lastName.Trim();

        if (normalizedFirstName.Length > 100)
            throw new ArgumentException("First name is too long (max 100 characters)", nameof(firstName));

        if (normalizedLastName.Length > 100)
            throw new ArgumentException("Last name is too long (max 100 characters)", nameof(lastName));

        var hasChanges = FirstName != normalizedFirstName ||
                        LastName != normalizedLastName ||
                        PhoneNumber != phoneNumber ||
                        Address != address;

        if (!hasChanges)
            return;

        FirstName = normalizedFirstName;
        LastName = normalizedLastName;
        PhoneNumber = phoneNumber;
        Address = address;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new CustomerProfileUpdated(
            Id,
            FirstName,
            LastName,
            PhoneNumber,
            Address,
            UpdatedAtUtc));
    }

    /// <summary>
    /// Updates the customer's driver's license.
    /// </summary>
    public void UpdateDriversLicense(DriversLicense newLicense)
    {
        ValidateDriversLicense(newLicense, DateOfBirth);

        if (DriversLicense == newLicense)
            return;

        var oldLicense = DriversLicense;
        DriversLicense = newLicense;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new DriversLicenseUpdated(
            Id,
            oldLicense,
            newLicense,
            UpdatedAtUtc));
    }

    /// <summary>
    /// Changes the customer's account status.
    /// </summary>
    public void ChangeStatus(CustomerStatus newStatus, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason, nameof(reason));

        if (Status == newStatus)
            return;

        var oldStatus = Status;
        Status = newStatus;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new CustomerStatusChanged(
            Id,
            oldStatus,
            newStatus,
            reason,
            UpdatedAtUtc));
    }

    /// <summary>
    /// Updates the customer's email address.
    /// </summary>
    public void UpdateEmail(Email newEmail)
    {
        if (Email == newEmail)
            return;

        var oldEmail = Email;
        Email = newEmail;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new CustomerEmailChanged(
            Id,
            oldEmail,
            newEmail,
            UpdatedAtUtc));
    }

    /// <summary>
    /// Activates the customer account.
    /// </summary>
    public void Activate(string reason = "Account activated")
    {
        if (Status == CustomerStatus.Active)
            return;

        ChangeStatus(CustomerStatus.Active, reason);
    }

    /// <summary>
    /// Suspends the customer account (temporary).
    /// </summary>
    public void Suspend(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason, nameof(reason));

        if (Status == CustomerStatus.Suspended)
            return;

        ChangeStatus(CustomerStatus.Suspended, reason);
    }

    /// <summary>
    /// Blocks the customer account (more severe than suspension).
    /// </summary>
    public void Block(string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason, nameof(reason));

        if (Status == CustomerStatus.Blocked)
            return;

        ChangeStatus(CustomerStatus.Blocked, reason);
    }

    /// <summary>
    /// Checks if the customer can make reservations.
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
    /// Checks if the customer can make a reservation for a specific rental period.
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
    /// Gets the customer's full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Gets the customer's current age in years.
    /// </summary>
    public int Age
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DateOfBirth.Year;

            // Adjust if birthday hasn't occurred this year
            if (DateOfBirth.AddYears(age) > today)
                age--;

            return age;
        }
    }

    /// <summary>
    /// Validates that the customer meets the minimum age requirement.
    /// </summary>
    private static void ValidateAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;

        // Adjust if birthday hasn't occurred this year
        if (dateOfBirth.AddYears(age) > today)
            age--;

        if (age < MinimumAgeYears)
        {
            throw new ArgumentException(
                $"Customer must be at least {MinimumAgeYears} years old. Current age: {age}",
                nameof(dateOfBirth));
        }

        // Sanity check - no one over 150 years old
        if (age > 150)
        {
            throw new ArgumentException(
                "Invalid date of birth - age cannot exceed 150 years",
                nameof(dateOfBirth));
        }

        // Date of birth cannot be in the future
        if (dateOfBirth > today)
        {
            throw new ArgumentException(
                "Date of birth cannot be in the future",
                nameof(dateOfBirth));
        }
    }

    /// <summary>
    /// Validates the driver's license against business rules.
    /// </summary>
    private static void ValidateDriversLicense(DriversLicense license, DateOnly dateOfBirth)
    {
        // License must be currently valid
        if (!license.IsValid())
        {
            throw new ArgumentException(
                $"Driver's license is expired. Expiry date: {license.ExpiryDate}",
                nameof(license));
        }

        // License must have minimum validity remaining
        if (license.DaysUntilExpiry() < MinimumLicenseValidityDays)
        {
            throw new ArgumentException(
                $"Driver's license must be valid for at least {MinimumLicenseValidityDays} days. " +
                $"Days remaining: {license.DaysUntilExpiry()}",
                nameof(license));
        }

        // License issue date must be after 18th birthday (with some tolerance)
        var minimumLicenseIssueDate = dateOfBirth.AddYears(MinimumAgeYears - 1);
        if (license.IssueDate < minimumLicenseIssueDate)
        {
            throw new ArgumentException(
                $"Driver's license issue date seems inconsistent with date of birth. " +
                $"License issued: {license.IssueDate}, Date of birth: {dateOfBirth}",
                nameof(license));
        }
    }
}

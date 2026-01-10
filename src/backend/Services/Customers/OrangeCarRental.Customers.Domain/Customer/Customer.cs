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
    ///     While the minimum driving age is 18, most German car rental companies require 21+.
    /// </summary>
    public const int MinimumRentalAgeYears = 21;

    /// <summary>
    ///     Minimum age required to register as a customer (can book, but rental eligibility checked separately).
    /// </summary>
    public const int MinimumRegistrationAgeYears = 18;

    /// <summary>
    ///     Minimum years the driver's license must be held before renting.
    /// </summary>
    public const int MinimumLicenseHeldYears = 1;

    /// <summary>
    ///     Minimum days before license expiry to allow rentals.
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
        CustomerType = CustomerType.Individual;
        RegisteredAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new CustomerRegistered(
            Id,
            Name,
            Email,
            PhoneNumber,
            DateOfBirth,
            Address,
            DriversLicense,
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
    public CustomerType CustomerType { get; init; }
    public CompanyName? CompanyName { get; init; }
    public VATId? VATId { get; init; }
    public PaymentTerms? PaymentTerms { get; init; }
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
    ///     Gets whether this is a business customer.
    /// </summary>
    public bool IsBusinessCustomer => CustomerType == CustomerType.Business;

    /// <summary>
    ///     Registers a new customer with German market validation.
    /// </summary>
    public static Customer Register(
        CustomerName name,
        Email email,
        PhoneNumber phoneNumber,
        BirthDate dateOfBirth,
        Address address,
        DriversLicense driversLicense)
    {
        // Validate age - must be 18+ to register in Germany
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
        CustomerType? customerType = null,
        CompanyName? companyName = null,
        VATId? vatId = null,
        PaymentTerms? paymentTerms = null,
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
            CustomerType = customerType ?? CustomerType,
            CompanyName = companyName ?? CompanyName,
            VATId = vatId ?? VATId,
            PaymentTerms = paymentTerms ?? PaymentTerms,
            RegisteredAtUtc = registeredAtUtc ?? RegisteredAtUtc,
            UpdatedAtUtc = updatedAtUtc ?? UpdatedAtUtc
        };
    }

    /// <summary>
    ///     Updates the customer's profile information.
    ///     Returns a new instance with updated values (immutable pattern).
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
            name: name,
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
    ///     Returns a new instance with updated license (immutable pattern).
    /// </summary>
    public Customer UpdateDriversLicense(DriversLicense newLicense)
    {
        ValidateDriversLicense(newLicense, DateOfBirth.Value);

        if (DriversLicense == newLicense)
            return this;

        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            driversLicense: newLicense,
            updatedAtUtc: now);

        updated.AddDomainEvent(new DriversLicenseUpdated(
            Id,
            DriversLicense,
            newLicense,
            now));

        return updated;
    }

    /// <summary>
    ///     Changes the customer's account status.
    ///     Returns a new instance with updated status (immutable pattern).
    /// </summary>
    public Customer ChangeStatus(CustomerStatus newStatus, string reason)
    {
        Ensure.That(reason, nameof(reason))
            .IsNotNullOrWhiteSpace();

        if (Status == newStatus)
            return this;

        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            status: newStatus,
            updatedAtUtc: now);

        updated.AddDomainEvent(new CustomerStatusChanged(
            Id,
            Status,
            newStatus,
            reason,
            now));

        return updated;
    }

    /// <summary>
    ///     Updates the customer's email address.
    ///     Returns a new instance with updated email (immutable pattern).
    /// </summary>
    public Customer UpdateEmail(Email newEmail)
    {
        if (Email == newEmail)
            return this;

        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            email: newEmail,
            updatedAtUtc: now);

        updated.AddDomainEvent(new CustomerEmailChanged(
            Id,
            Email,
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
    ///     Upgrades the customer to a business account (Geschäftskunde).
    ///     Returns a new instance with business customer details (immutable pattern).
    /// </summary>
    public Customer UpgradeToBusiness(
        CompanyName companyName,
        VATId vatId,
        PaymentTerms paymentTerms)
    {
        Ensure.That(CustomerType, nameof(CustomerType))
            .ThrowInvalidOperationIf(CustomerType == CustomerType.Business, "Customer is already a business customer");

        Ensure.That(vatId, nameof(vatId))
            .ThrowIf(!vatId.IsGerman, "Only German VAT IDs are supported");

        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            customerType: CustomerType.Business,
            companyName: companyName,
            vatId: vatId,
            paymentTerms: paymentTerms,
            updatedAtUtc: now);

        updated.AddDomainEvent(new CustomerUpgradedToBusiness(
            Id,
            companyName,
            vatId,
            paymentTerms,
            now));

        return updated;
    }

    /// <summary>
    ///     Updates the business customer details.
    ///     Returns a new instance with updated business details (immutable pattern).
    /// </summary>
    public Customer UpdateBusinessDetails(
        CompanyName companyName,
        VATId vatId,
        PaymentTerms paymentTerms)
    {
        Ensure.That(CustomerType, nameof(CustomerType))
            .ThrowInvalidOperationIf(CustomerType != CustomerType.Business, "Customer is not a business customer");

        Ensure.That(vatId, nameof(vatId))
            .ThrowIf(!vatId.IsGerman, "Only German VAT IDs are supported");

        // Check for changes
        var hasChanges = CompanyName != companyName ||
                         VATId != vatId ||
                         PaymentTerms != paymentTerms;

        if (!hasChanges)
            return this;

        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            companyName: companyName,
            vatId: vatId,
            paymentTerms: paymentTerms,
            updatedAtUtc: now);

        updated.AddDomainEvent(new BusinessDetailsUpdated(
            Id,
            companyName,
            vatId,
            paymentTerms,
            now));

        return updated;
    }

    /// <summary>
    ///     Checks if the customer can make reservations.
    /// </summary>
    public bool CanMakeReservation()
    {
        if (Status != CustomerStatus.Active)
            return false;

        if (!DriversLicense.IsValid())
            return false;

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

        if (!DriversLicense.IsValidOn(startDate) || !DriversLicense.IsValidOn(endDate))
            return false;

        return true;
    }

    private static void ValidateAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.AddYears(age) > today)
            age--;

        Ensure.That(dateOfBirth, nameof(dateOfBirth))
            .ThrowIf(age < MinimumRegistrationAgeYears, $"Customer must be at least {MinimumRegistrationAgeYears} years old to register. Current age: {age}");

        Ensure.That(dateOfBirth, nameof(dateOfBirth))
            .ThrowIf(age > 150, "Invalid date of birth - age cannot exceed 150 years");

        Ensure.That(dateOfBirth, nameof(dateOfBirth))
            .ThrowIf(dateOfBirth > today, "Date of birth cannot be in the future");
    }

    private static void ValidateDriversLicense(DriversLicense license, DateOnly dateOfBirth)
    {
        Ensure.That(license, nameof(license))
            .ThrowIf(!license.IsValid(), $"Driver's license is expired. Expiry date: {license.ExpiryDate}");

        Ensure.That(license, nameof(license))
            .ThrowIf(license.DaysUntilExpiry() < MinimumLicenseValidityDays,
                $"Driver's license must be valid for at least {MinimumLicenseValidityDays} days. Days remaining: {license.DaysUntilExpiry()}");

        var minimumLicenseIssueDate = dateOfBirth.AddYears(MinimumRegistrationAgeYears - 1);
        Ensure.That(license, nameof(license))
            .ThrowIf(license.IssueDate < minimumLicenseIssueDate,
                $"Driver's license issue date seems inconsistent with date of birth. License issued: {license.IssueDate}, Date of birth: {dateOfBirth}");
    }

    /// <summary>
    ///     Validates rental eligibility for a specific rental period according to German requirements.
    /// </summary>
    public RentalEligibilityResult ValidateRentalEligibility(DateOnly startDate)
    {
        var issues = new List<string>();

        // Check account status
        if (Status != CustomerStatus.Active)
            issues.Add($"Account is not active (status: {Status})");

        // Check age requirement (21+)
        var ageOnRentalDate = CalculateAge(DateOfBirth.Value, startDate);
        if (ageOnRentalDate < MinimumRentalAgeYears)
            issues.Add($"Must be at least {MinimumRentalAgeYears} years old to rent (age on rental date: {ageOnRentalDate})");

        // Check driver's license
        var licenseValidation = DriversLicense.ValidateForGermanRental(startDate);
        if (!licenseValidation.IsValid)
            issues.AddRange(licenseValidation.Issues);

        return new RentalEligibilityResult(issues.Count == 0, issues);
    }

    private static int CalculateAge(DateOnly dateOfBirth, DateOnly asOfDate)
    {
        var age = asOfDate.Year - dateOfBirth.Year;
        if (dateOfBirth.AddYears(age) > asOfDate)
            age--;
        return age;
    }
}

/// <summary>
///     Result of rental eligibility validation.
/// </summary>
/// <param name="IsEligible">Whether the customer is eligible to rent.</param>
/// <param name="Issues">List of eligibility issues if any.</param>
public sealed record RentalEligibilityResult(bool IsEligible, IReadOnlyList<string> Issues)
{
    public static RentalEligibilityResult Eligible => new(true, []);
}

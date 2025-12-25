using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.EventSourcing;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer.Events;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Customer aggregate root (event-sourced).
///     Represents a customer in the Orange Car Rental system with German market-specific validation.
///     Enforces business rules such as minimum age (18+), valid driver's license, and GDPR compliance.
/// </summary>
public sealed class Customer : EventSourcedAggregate<CustomerQueryModel, CustomerIdentifier>
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

    /// <summary>
    ///     Gets the customer's name.
    /// </summary>
    public CustomerName? Name => State.Name;

    /// <summary>
    ///     Gets the customer's email.
    /// </summary>
    public Email? Email => State.Email;

    /// <summary>
    ///     Gets the customer's phone number.
    /// </summary>
    public PhoneNumber? PhoneNumber => State.PhoneNumber;

    /// <summary>
    ///     Gets the customer's date of birth.
    /// </summary>
    public BirthDate? DateOfBirth => State.DateOfBirth;

    /// <summary>
    ///     Gets the customer's address.
    /// </summary>
    public Address? Address => State.Address;

    /// <summary>
    ///     Gets the customer's driver's license.
    /// </summary>
    public DriversLicense? DriversLicense => State.DriversLicense;

    /// <summary>
    ///     Gets the customer's account status.
    /// </summary>
    public CustomerStatus Status => State.Status;

    /// <summary>
    ///     Gets when the customer registered.
    /// </summary>
    public DateTime RegisteredAtUtc => State.RegisteredAtUtc;

    /// <summary>
    ///     Gets when the customer was last updated.
    /// </summary>
    public DateTime UpdatedAtUtc => State.UpdatedAtUtc;

    /// <summary>
    ///     Gets the customer's full name.
    /// </summary>
    public string FullName => State.FullName;

    /// <summary>
    ///     Gets the customer's formal name with salutation.
    /// </summary>
    public string FormalName => State.FormalName;

    /// <summary>
    ///     Gets the customer's current age in years.
    /// </summary>
    public int Age => State.Age;

    /// <summary>
    ///     Registers a new customer with German market validation.
    /// </summary>
    public void Register(
        CustomerName name,
        Email email,
        PhoneNumber phoneNumber,
        BirthDate dateOfBirth,
        Address address,
        DriversLicense driversLicense)
    {
        EnsureDoesNotExist();

        // Validate age - must be 18+ to rent in Germany
        ValidateAge(dateOfBirth.Value);

        // Validate driver's license
        ValidateDriversLicense(driversLicense, dateOfBirth.Value);

        var now = DateTime.UtcNow;
        Apply(new CustomerRegistered(
            CustomerIdentifier.New(),
            name,
            email,
            phoneNumber,
            dateOfBirth,
            address,
            driversLicense,
            now));
    }

    /// <summary>
    ///     Updates the customer's profile information.
    /// </summary>
    public void UpdateProfile(
        CustomerName name,
        PhoneNumber phoneNumber,
        Address address)
    {
        EnsureExists();

        var hasChanges = State.Name?.FirstName.Value != name.FirstName.Value ||
                         State.Name?.LastName.Value != name.LastName.Value ||
                         State.Name?.Salutation != name.Salutation ||
                         State.PhoneNumber != phoneNumber ||
                         State.Address != address;

        if (!hasChanges)
            return;

        var now = DateTime.UtcNow;
        Apply(new CustomerProfileUpdated(
            Id,
            name,
            phoneNumber,
            address,
            now));
    }

    /// <summary>
    ///     Updates the customer's driver's license.
    /// </summary>
    public void UpdateDriversLicense(DriversLicense newLicense)
    {
        EnsureExists();

        if (State.DateOfBirth is null)
            throw new InvalidOperationException("Customer date of birth is required");

        ValidateDriversLicense(newLicense, State.DateOfBirth.Value);

        if (State.DriversLicense == newLicense)
            return;

        var oldLicense = State.DriversLicense ??
            throw new InvalidOperationException("Customer has no existing driver's license");

        var now = DateTime.UtcNow;
        Apply(new DriversLicenseUpdated(
            Id,
            oldLicense,
            newLicense,
            now));
    }

    /// <summary>
    ///     Changes the customer's account status.
    /// </summary>
    public void ChangeStatus(CustomerStatus newStatus, string reason)
    {
        EnsureExists();

        Ensure.That(reason, nameof(reason))
            .IsNotNullOrWhiteSpace();

        if (State.Status == newStatus)
            return;

        var now = DateTime.UtcNow;
        Apply(new CustomerStatusChanged(
            Id,
            State.Status,
            newStatus,
            reason,
            now));
    }

    /// <summary>
    ///     Updates the customer's email address.
    /// </summary>
    public void UpdateEmail(Email newEmail)
    {
        EnsureExists();

        if (State.Email == newEmail)
            return;

        var oldEmail = State.Email ??
            throw new InvalidOperationException("Customer has no existing email");

        var now = DateTime.UtcNow;
        Apply(new CustomerEmailChanged(
            Id,
            oldEmail,
            newEmail,
            now));
    }

    /// <summary>
    ///     Activates the customer account.
    /// </summary>
    public void Activate(string reason = "Account activated")
    {
        if (State.Status == CustomerStatus.Active)
            return;

        ChangeStatus(CustomerStatus.Active, reason);
    }

    /// <summary>
    ///     Suspends the customer account (temporary).
    /// </summary>
    public void Suspend(string reason)
    {
        Ensure.That(reason, nameof(reason))
            .IsNotNullOrWhiteSpace();

        if (State.Status == CustomerStatus.Suspended)
            return;

        ChangeStatus(CustomerStatus.Suspended, reason);
    }

    /// <summary>
    ///     Blocks the customer account (more severe than suspension).
    /// </summary>
    public void Block(string reason)
    {
        Ensure.That(reason, nameof(reason))
            .IsNotNullOrWhiteSpace();

        if (State.Status == CustomerStatus.Blocked)
            return;

        ChangeStatus(CustomerStatus.Blocked, reason);
    }

    /// <summary>
    ///     Checks if the customer can make reservations.
    /// </summary>
    public bool CanMakeReservation()
    {
        if (State.Status != CustomerStatus.Active)
            return false;

        if (!State.DriversLicense.HasValue)
            return false;

        var license = State.DriversLicense.Value;
        if (!license.IsValid())
            return false;

        if (license.DaysUntilExpiry() < MinimumLicenseValidityDays)
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

        if (!State.DriversLicense.HasValue)
            return false;

        var license = State.DriversLicense.Value;
        if (!license.IsValidOn(startDate) || !license.IsValidOn(endDate))
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
    /// <param name="startDate">The rental start date.</param>
    /// <returns>Detailed validation result.</returns>
    public RentalEligibilityResult ValidateRentalEligibility(DateOnly startDate)
    {
        var issues = new List<string>();

        // Check account status
        if (State.Status != CustomerStatus.Active)
            issues.Add($"Account is not active (status: {State.Status})");

        // Check age requirement (21+)
        if (State.DateOfBirth.HasValue)
        {
            var ageOnRentalDate = CalculateAge(State.DateOfBirth.Value, startDate);
            if (ageOnRentalDate < MinimumRentalAgeYears)
                issues.Add($"Must be at least {MinimumRentalAgeYears} years old to rent (age on rental date: {ageOnRentalDate})");
        }
        else
        {
            issues.Add("Date of birth is required");
        }

        // Check driver's license
        if (State.DriversLicense.HasValue)
        {
            var license = State.DriversLicense.Value;
            var licenseValidation = license.ValidateForGermanRental(startDate);

            if (!licenseValidation.IsValid)
                issues.AddRange(licenseValidation.Issues);
        }
        else
        {
            issues.Add("Driver's license is required");
        }

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

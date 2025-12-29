namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Constants for customer sort field names.
///     Use these instead of magic strings when specifying sort fields.
/// </summary>
public static class CustomerSortFields
{
    // Name fields
    public const string FirstName = "firstname";
    public const string FirstNameAlt = "first_name";
    public const string LastName = "lastname";
    public const string LastNameAlt = "last_name";

    // Contact fields
    public const string Email = "email";
    public const string PhoneNumber = "phonenumber";
    public const string PhoneNumberAlt = "phone_number";
    public const string Phone = "phone";

    // Personal info
    public const string DateOfBirth = "dateofbirth";
    public const string DateOfBirthAlt = "date_of_birth";
    public const string BirthDate = "birthdate";

    // Address fields
    public const string City = "city";
    public const string PostalCode = "postalcode";
    public const string PostalCodeAlt = "postal_code";
    public const string Zip = "zip";

    // Status and dates
    public const string Status = "status";
    public const string RegisteredAt = "registeredat";
    public const string RegisteredAtAlt = "registered_at";
    public const string Created = "created";
    public const string CreatedAt = "createdat";
    public const string UpdatedAt = "updatedat";
    public const string UpdatedAtAlt = "updated_at";
    public const string Modified = "modified";
    public const string ModifiedAt = "modifiedat";

    // License fields
    public const string LicenseExpiry = "licenseexpiry";
    public const string LicenseExpiryAlt = "license_expiry";
    public const string ExpiryDate = "expirydate";

    /// <summary>
    ///     Default sort field for customer queries.
    /// </summary>
    public const string Default = RegisteredAt;

    /// <summary>
    ///     All valid sort field names.
    /// </summary>
    public static readonly IReadOnlyList<string> All =
    [
        FirstName, FirstNameAlt, LastName, LastNameAlt,
        Email, PhoneNumber, PhoneNumberAlt, Phone,
        DateOfBirth, DateOfBirthAlt, BirthDate,
        City, PostalCode, PostalCodeAlt, Zip,
        Status, RegisteredAt, RegisteredAtAlt, Created, CreatedAt,
        UpdatedAt, UpdatedAtAlt, Modified, ModifiedAt,
        LicenseExpiry, LicenseExpiryAlt, ExpiryDate
    ];
}

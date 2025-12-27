namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;

/// <summary>
/// Common customer test data.
/// Based on German naming conventions and addresses.
/// </summary>
public static class TestCustomer
{
    /// <summary>
    /// Max Mustermann - the German equivalent of John Doe.
    /// </summary>
    public static class MaxMustermann
    {
        public const string FirstName = "Max";
        public const string LastName = "Mustermann";
        public const string FullName = "Max Mustermann";
        public const string Email = "max.mustermann@example.de";
        public const string Phone = "+49 30 12345678";
        public const string Street = "Musterstraße 123";
        public const string PostalCode = "10115";
        public const string City = "Berlin";
        public const string Country = "Deutschland";

        public static DateOnly DateOfBirth => TestDates.Adult30;
        public static DateOnly LicenseIssueDate => TestDates.LicenseIssued5YearsAgo;
        public static DateOnly LicenseExpiryDate => TestDates.LicenseExpiry5Years;
        public const string LicenseNumber = "B072RRE2I55";
        public const string LicenseCountry = "DE";
    }

    /// <summary>
    /// Erika Musterfrau - female test customer.
    /// </summary>
    public static class ErikaMusterfrau
    {
        public const string FirstName = "Erika";
        public const string LastName = "Musterfrau";
        public const string FullName = "Erika Musterfrau";
        public const string Email = "erika.musterfrau@example.de";
        public const string Phone = "+49 89 98765432";
        public const string Street = "Beispielweg 45";
        public const string PostalCode = "80331";
        public const string City = "München";
        public const string Country = "Deutschland";

        public static DateOnly DateOfBirth => TestDates.Adult25;
        public static DateOnly LicenseIssueDate => TestDates.LicenseIssued5YearsAgo;
        public static DateOnly LicenseExpiryDate => TestDates.LicenseExpiry5Years;
        public const string LicenseNumber = "M089XYZ1234";
        public const string LicenseCountry = "DE";
    }

    /// <summary>
    /// Business customer test data.
    /// </summary>
    public static class BusinessCustomer
    {
        public const string CompanyName = "Musterfirma GmbH";
        public const string VatId = "DE123456789";
        public const string Street = "Industriestraße 1";
        public const string PostalCode = "60311";
        public const string City = "Frankfurt am Main";
        public const string Country = "Deutschland";
        public const string ContactName = "Hans Schmidt";
        public const string ContactEmail = "h.schmidt@musterfirma.de";
        public const string ContactPhone = "+49 69 11223344";
    }
}

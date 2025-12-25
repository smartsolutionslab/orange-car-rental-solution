# German Market Requirements

This document outlines specific requirements for operating a car rental business in the German market.

## Legal & Compliance

### 1. GDPR (DSGVO) Compliance

**Data Protection Requirements:**
- ✅ Explicit consent for data collection
- ✅ Right to access personal data
- ✅ Right to erasure (data anonymization in event store)
- ✅ Right to data portability
- ✅ Privacy policy (Datenschutzerklärung)
- ✅ Cookie consent banner
- ✅ Data processing agreements with third parties

**Implementation:**
```csharp
// Customer aggregate with GDPR support
public sealed class Customer : AggregateRoot<CustomerId>
{
    public void AnonymizePersonalData()
    {
        // GDPR Right to erasure
        Name = PersonName.Anonymized();
        Email = EmailAddress.Anonymized();
        DateOfBirth = DateOfBirth.Anonymized();
        Address = Address.Anonymized();

        AddDomainEvent(new CustomerDataAnonymized(Id));
    }
}
```

**Required Pages:**
- Datenschutzerklärung (Privacy Policy)
- Impressum (Legal Notice)
- Allgemeine Geschäftsbedingungen (Terms & Conditions)
- Widerrufsbelehrung (Right of Withdrawal)

### 2. Impressum (Legal Notice)

**Required Information:**
- Company name and legal form
- Full address
- Contact information (phone, email)
- Trade register number (Handelsregisternummer)
- VAT ID (Umsatzsteuer-Identifikationsnummer)
- Responsible person (Vertretungsberechtigter)
- Professional association
- Regulatory authority

**Implementation:**
- Static page in both frontend applications
- Easily accessible footer link
- Content managed via CMS or configuration

### 3. VAT (Mehrwertsteuer) Handling

**German VAT Requirements:**
- Standard rate: 19%
- Reduced rate: 7% (not applicable for car rentals)
- VAT must be shown separately on invoices
- VAT ID required for business customers (B2B)

**Value Object Extension:**
```csharp
public sealed record Money(decimal NetAmount, decimal VatAmount, Currency Currency)
{
    public decimal GrossAmount => NetAmount + VatAmount;
    public decimal VatRate => NetAmount > 0 ? VatAmount / NetAmount : 0;

    public static Money CreateWithVat(decimal netAmount, decimal vatRate, Currency currency)
    {
        var vatAmount = netAmount * vatRate;
        return new Money(netAmount, vatAmount, currency);
    }
}

// Usage for German market
var dailyRate = Money.CreateWithVat(100m, 0.19m, new Currency("EUR"));
// Net: 100€, VAT: 19€, Gross: 119€
```

### 4. Invoice Requirements (Rechnungsstellung)

**Mandatory Invoice Information:**
- Invoice number (consecutive)
- Invoice date
- Delivery/service date
- Seller details (see Impressum)
- Customer details
- VAT ID (if B2B)
- Item description
- Net amount per item
- VAT rate and amount
- Gross total
- Payment terms

**Implementation:**
- Generate PDF invoices
- Store in event store
- Email to customer
- Archive for 10 years (German tax law requirement)

## Language & Localization

### 1. German Language Support

**Primary Language:**
- German (de-DE) as default
- English (en-US) as secondary option

**Implementation:**
```typescript
// Angular i18n
export const translations = {
  'de-DE': {
    'vehicle.search.title': 'Fahrzeugsuche',
    'vehicle.search.pickup': 'Abholstation',
    'vehicle.search.return': 'Rückgabestation',
    'booking.renter.title': 'Mieter',
    'booking.renter.firstName': 'Vorname',
    'booking.renter.lastName': 'Nachname',
    // ...
  },
  'en-US': {
    'vehicle.search.title': 'Vehicle Search',
    'vehicle.search.pickup': 'Pickup Location',
    // ...
  }
};
```

### 2. Date & Time Formats

**German Formats:**
- Date: DD.MM.YYYY (e.g., 28.10.2025)
- Time: HH:mm (24-hour format)
- Currency: 1.234,56 € (thousand separator: dot, decimal: comma)

**Value Object:**
```csharp
public sealed record FormattedDate(DateTime Value, CultureInfo Culture)
{
    public string ToGermanFormat() => Value.ToString("dd.MM.yyyy", new CultureInfo("de-DE"));
    public string ToIsoFormat() => Value.ToString("yyyy-MM-dd");
}
```

### 3. Address Format

**German Address Structure:**
```
Title FirstName LastName
Street HouseNumber
PostalCode City
Country
```

**Validation:**
- PostalCode: 5 digits (e.g., 10115)
- Phone: +49 (country code) + area code + number
- Valid German states (Bundesländer)

```csharp
public sealed record GermanPostalCode : PostalCode
{
    public GermanPostalCode(string value) : base(value)
    {
        if (!Regex.IsMatch(value, @"^\d{5}$"))
            throw new ArgumentException("German postal code must be 5 digits");
    }
}
```

## Payment Methods

### 1. Common German Payment Methods

**Must Support:**
- ✅ SEPA Direct Debit (SEPA-Lastschrift)
- ✅ Credit Card (Visa, Mastercard)
- ✅ Debit Card (EC-Karte / Girocard)
- ✅ PayPal

**Nice to Have:**
- Klarna (invoice payment)
- Sofortüberweisung
- Apple Pay / Google Pay

### 2. SEPA Direct Debit

**Requirements:**
- SEPA mandate (SEPA-Lastschriftmandat)
- Mandate reference number
- Creditor ID (Gläubiger-Identifikationsnummer)
- Pre-notification (Vorabankündigung) - at least 5 days before debit

**Value Objects (Implemented):**
```csharp
// IBAN with MOD-97 check digit validation (ISO 13616)
public sealed partial record IBAN : IValueObject
{
    public string Value { get; }
    public string CountryCode => Value[..2];
    public bool IsGerman => CountryCode == "DE";
    public string Formatted => FormatIBAN(Value);  // DE89 3704 0044 0532 0130 00

    public static IBAN Create(string value)
    {
        // Validates format and MOD-97 check digits
        // German IBANs must be exactly 22 characters
    }
}

// BIC/SWIFT validation (ISO 9362)
public sealed partial record BIC : IValueObject
{
    public string BankCode => Value[..4];
    public string CountryCode => Value[4..6];
    public bool IsGerman => CountryCode == "DE";
}

// SEPA Mandate aggregate with status management
public sealed class SepaMandate : AggregateRoot<SepaMandateIdentifier>
{
    public const string CreditorId = "DE98ZZZ09999999999";
    public MandateReference MandateReference { get; init; }
    public IBAN IBAN { get; init; }
    public BIC BIC { get; init; }
    public string AccountHolder { get; init; }
    public MandateStatus Status { get; init; }

    public SepaMandate RecordUsage();  // Track when mandate is used
    public SepaMandate Revoke();       // Customer revokes mandate
    public bool IsExpiredDueToInactivity();  // 36-month SEPA rule
}
```

### 3. Payment Gateway Integration

**German-friendly gateways:**
- Stripe (supports SEPA, Giropay)
- Adyen (strong European presence)
- Mollie (European payment provider)

## Vehicle & Rental Specifics

### 1. German Driving License

**Validation Requirements (Implemented):**
- Valid German (EU) driving license required
- Minimum age: 21 years for car rental
- License held for at least 1 year
- International licenses require International Driving Permit (warning issued)

**Value Object (Implemented):**
```csharp
public readonly record struct DriversLicense : IValueObject
{
    public string LicenseNumber { get; }
    public string IssueCountry { get; }
    public DateOnly IssueDate { get; }
    public DateOnly ExpiryDate { get; }

    public bool IsEuLicense();  // Recognizes all 27 EU countries
    public bool HasBeenHeldForYears(int years, DateOnly? asOfDate = null);
    public int YearsHeld(DateOnly? asOfDate = null);
    public bool IsValidForGermanRental(DateOnly rentalDate);
    public LicenseValidationResult ValidateForGermanRental(DateOnly rentalDate);
}

// Customer aggregate enforces rental requirements
public sealed class Customer : EventSourcedAggregate<...>
{
    public const int MinimumRentalAgeYears = 21;
    public const int MinimumRegistrationAgeYears = 18;
    public const int MinimumLicenseHeldYears = 1;

    public RentalEligibilityResult ValidateRentalEligibility(DateOnly startDate);
}
```

### 2. Insurance Requirements (Implemented)

**Mandatory Coverage (Pflichtversicherung):**
- Haftpflichtversicherung (Liability insurance)
- Vollkasko (Comprehensive coverage) - optional but recommended
- Teilkasko (Partial coverage)

**Deductible (Selbstbeteiligung):**
- Standard: 1.000€
- Reduced: 500€
- Full coverage: 0€ (additional cost)

**Value Object (Implemented):**
```csharp
public sealed record InsurancePackage : IValueObject
{
    public InsuranceType Type { get; }
    public Money Deductible { get; }
    public Money DailySurcharge { get; }
    public bool IncludesTheftProtection { get; }
    public bool IncludesGlassAndTires { get; }
    public bool IncludesPersonalAccident { get; }
    public bool IsZeroDeductible { get; }

    // 4 predefined German market packages
    public static readonly InsurancePackage Basic = new(
        InsuranceType.Haftpflicht,
        Money.EuroGross(2500.00m), // High liability
        Money.Zero(Currency.EUR)); // Included in base price

    public static readonly InsurancePackage Standard = new(
        InsuranceType.Teilkasko,
        Money.EuroGross(1000.00m),
        Money.EuroGross(8.00m));

    public static readonly InsurancePackage Comfort = new(
        InsuranceType.Vollkasko,
        Money.EuroGross(500.00m),
        Money.EuroGross(15.00m));

    public static readonly InsurancePackage Premium = new(
        InsuranceType.VollkaskoZeroDeductible,
        Money.Zero(Currency.EUR),
        Money.EuroGross(25.00m));

    public Money CalculateCost(int rentalDays);
    public string GetGermanDisplayName();  // "Vollkasko (SB 500€)"
    public string GetEnglishDisplayName(); // "Comprehensive (€500 excess)"
    public string GetCoverageDescription();
    public static IReadOnlyList<InsurancePackage> GetAllPackages();
}
```

### 3. Fuel Policy

**Common in Germany:**
- Full-to-Full (Voll-zu-Voll) - most common
- Full-to-Empty (Voll-zu-Leer) - less common
- Fair Fuel Policy (Faire Tankregelung)

```csharp
public sealed record FuelPolicy
{
    public static readonly FuelPolicy FullToFull = new("FullToFull");
    public static readonly FuelPolicy FullToEmpty = new("FullToEmpty");

    public string Value { get; }
    private FuelPolicy(string value) => Value = value;
}
```

### 4. Vehicle Categories (German Market)

**Common Categories:**
- Kleinwagen (Small Car) - VW Up, Fiat 500
- Kompaktklasse (Compact) - VW Golf, Opel Astra
- Mittelklasse (Mid-size) - VW Passat, BMW 3er
- Oberklasse (Full-size) - BMW 5er, Mercedes E-Klasse
- Van/Bus - VW T6, Mercedes Vito
- SUV - VW Tiguan, BMW X3

## Additional Features for German Market

### 1. Kilometer Packages (Implemented)

**Typical Offerings:**
- 100 km/day included
- 200 km/day package
- Unlimited kilometers
- Additional km charged at 0,20€/km

**Value Object (Implemented):**
```csharp
public sealed record KilometerPackage : IValueObject
{
    public static readonly KilometerPackage Limited100 = new(
        KilometerPackageType.Limited100, 100, Money.EuroGross(0.20m));
    public static readonly KilometerPackage Limited200 = new(
        KilometerPackageType.Limited200, 200, Money.EuroGross(0.20m));
    public static readonly KilometerPackage Unlimited = new(
        KilometerPackageType.Unlimited, null, null);

    public KilometerPackageType Type { get; }
    public int? DailyLimitKm { get; }
    public Money? AdditionalKmRate { get; }
    public bool IsUnlimited => Type == KilometerPackageType.Unlimited;

    public int? GetTotalAllowance(int days);
    public Money CalculateAdditionalCharge(int totalDays, int kilometersDriven);
    public string GetGermanDisplayName();  // "100 km/Tag inklusive"
    public string GetEnglishDisplayName(); // "100 km/day included"
}
```

### 2. Additional Equipment (Extras) (Implemented)

**Common in Germany:**
- Navigationssystem (GPS)
- Kindersitz (Child seat) - various age groups
- Winterreifen (Winter tires) - mandatory Nov-Mar in certain conditions
- Schneeketten (Snow chains)
- Dachgepäckträger (Roof rack)
- Anhängerkupplung (Trailer hitch)

**Value Object (Implemented):**
```csharp
public sealed record VehicleExtra : IValueObject
{
    public VehicleExtraType Type { get; }
    public Money DailyRate { get; }
    public bool RequiresAdvanceBooking { get; }
    public bool IsPerRental { get; }

    // 12 predefined German market extras
    public static readonly VehicleExtra GPS = new(VehicleExtraType.GPS,
        Money.EuroGross(5.00m), requiresAdvanceBooking: false, isPerRental: false);
    public static readonly VehicleExtra ChildSeatInfant = new(VehicleExtraType.ChildSeatInfant,
        Money.EuroGross(8.00m), requiresAdvanceBooking: true, isPerRental: false);
    public static readonly VehicleExtra ChildSeatToddler = new(VehicleExtraType.ChildSeatToddler,
        Money.EuroGross(8.00m), requiresAdvanceBooking: true, isPerRental: false);
    public static readonly VehicleExtra BoosterSeat = new(VehicleExtraType.BoosterSeat,
        Money.EuroGross(5.00m), requiresAdvanceBooking: true, isPerRental: false);
    public static readonly VehicleExtra WinterTires = new(VehicleExtraType.WinterTires,
        Money.Zero(Currency.EUR), requiresAdvanceBooking: false, isPerRental: true);
    public static readonly VehicleExtra SnowChains = new(VehicleExtraType.SnowChains,
        Money.EuroGross(15.00m), requiresAdvanceBooking: true, isPerRental: true);
    public static readonly VehicleExtra RoofRack = new(VehicleExtraType.RoofRack,
        Money.EuroGross(10.00m), requiresAdvanceBooking: true, isPerRental: false);
    public static readonly VehicleExtra AdditionalDriver = new(VehicleExtraType.AdditionalDriver,
        Money.EuroGross(10.00m), requiresAdvanceBooking: false, isPerRental: false);
    public static readonly VehicleExtra CrossBorderPermit = new(VehicleExtraType.CrossBorderPermit,
        Money.EuroGross(15.00m), requiresAdvanceBooking: false, isPerRental: true);
    // ... plus SkiRack, BikeRack, TrailerHitch

    public Money CalculateCost(int rentalDays);
    public string GetGermanDisplayName();  // "Navigationssystem", "Babyschale (0-12 Monate)"
    public string GetEnglishDisplayName(); // "GPS Navigation", "Infant Car Seat"
    public static IReadOnlyList<VehicleExtra> GetAllExtras();
    public static VehicleExtra FromType(VehicleExtraType type);
}
```

### 3. Cross-Border Travel

**Restrictions:**
- Most rental companies allow EU travel
- Eastern European countries may require permission
- Additional insurance for certain countries
- Separate daily charge for cross-border

```csharp
public sealed record CrossBorderPolicy(
    bool AllowedInEU,
    HashSet<Country> RestrictedCountries,
    Money? DailySurcharge);
```

### 4. Business Customer (Geschäftskunde) Support (Implemented)

**B2B Features:**
- Corporate accounts
- VAT ID validation
- Framework agreements (Rahmenverträge)
- Invoice payment terms (Zahlungsziel: 14/30 days)
- Cost center allocation

**Value Objects (Implemented):**
```csharp
// German VAT ID (USt-IdNr.) with format validation
public readonly partial record struct VATId : IValueObject
{
    public string Value { get; }
    public string CountryCode => Value[..2];  // "DE"
    public string NumericPart => Value[2..];  // "123456789"
    public bool IsGerman => CountryCode == "DE";
    public string Formatted => $"{CountryCode} {NumericPart}";

    public static VATId Create(string value);  // Validates DE + 9 digits
    public static bool TryCreate(string value, out VATId vatId);
}

// Company name with German legal form detection
public readonly record struct CompanyName : IValueObject
{
    public string Value { get; }
    public bool HasGermanLegalForm();  // GmbH, AG, KG, OHG, UG, etc.
}

// Payment terms (Zahlungsziel)
public readonly record struct PaymentTerms : IValueObject
{
    public int DaysUntilDue { get; }
    public bool IsImmediate => DaysUntilDue == 0;

    public static readonly PaymentTerms Immediate = new(0);
    public static readonly PaymentTerms Net7 = new(7);
    public static readonly PaymentTerms Net14 = new(14);
    public static readonly PaymentTerms Net30 = new(30);
    public static readonly PaymentTerms Net60 = new(60);

    public DateOnly CalculateDueDate(DateOnly invoiceDate);
    public string GetGermanDisplayName();  // "Zahlungsziel 30 Tage"
}
```

**Customer Aggregate Extension:**
```csharp
public sealed class Customer : EventSourcedAggregate<...>
{
    public CustomerType CustomerType { get; }  // Individual or Business
    public CompanyName? CompanyName { get; }
    public VATId? VATId { get; }
    public PaymentTerms? PaymentTerms { get; }
    public bool IsBusinessCustomer { get; }

    public void UpgradeToBusiness(CompanyName, VATId, PaymentTerms);
    public void UpdateBusinessDetails(CompanyName, VATId, PaymentTerms);
}
```

## Accessibility & User Experience

### 1. Accessibility (Barrierefreiheit)

**Legal Requirement:**
- Barrierefreie-Informationstechnik-Verordnung (BITV 2.0)
- WCAG 2.1 Level AA compliance
- Screen reader support
- Keyboard navigation
- Sufficient color contrast

### 2. Cookie Consent

**Requirements:**
- Explicit consent required before setting non-essential cookies
- Granular control (Analytics, Marketing, Essential)
- Easy to decline
- "Reject All" option must be as prominent as "Accept All"

**Implementation:**
```typescript
// Use a GDPR-compliant cookie consent library
import { CookieConsent } from '@cookieconsent/angular';

export class AppComponent {
  cookieConfig = {
    categories: {
      essential: { enabled: true, readOnly: true },
      analytics: { enabled: false },
      marketing: { enabled: false }
    },
    language: {
      default: 'de',
      translations: {
        de: {
          consentModal: {
            title: 'Wir verwenden Cookies',
            description: 'Wir verwenden Cookies, um Ihre Erfahrung zu verbessern...',
            acceptAll: 'Alle akzeptieren',
            rejectAll: 'Alle ablehnen',
            settings: 'Einstellungen'
          }
        }
      }
    }
  };
}
```

## Technical Considerations

### 1. Email Templates

**German Language Templates:**
- Booking confirmation (Buchungsbestätigung)
- Cancellation confirmation (Stornierungsbestätigung)
- Reminder emails (Erinnerungen)
- Invoice (Rechnung)

### 2. Error Messages

**User-Friendly German Messages:**
```typescript
export const errorMessages = {
  'reservation.vehicle-unavailable': 'Das ausgewählte Fahrzeug ist für den gewünschten Zeitraum nicht verfügbar.',
  'validation.min-age': 'Sie müssen mindestens 21 Jahre alt sein, um ein Fahrzeug zu mieten.',
  'validation.invalid-postal-code': 'Bitte geben Sie eine gültige deutsche Postleitzahl ein (5 Ziffern).',
  'payment.failed': 'Die Zahlung konnte nicht durchgeführt werden. Bitte überprüfen Sie Ihre Zahlungsinformationen.',
};
```

### 3. Time Zones

**German Time Zone:**
- CET (Central European Time) / CEST (Central European Summer Time)
- UTC+1 / UTC+2 (daylight saving)

**Implementation:**
```csharp
public sealed record ZonedDateTime(DateTime UtcDateTime)
{
    public DateTime ToGermanTime()
    {
        var germanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(UtcDateTime, germanTimeZone);
    }
}
```

## Testing Checklist

### German Market Compliance Testing

- [ ] GDPR data anonymization works correctly
- [ ] Impressum page accessible from all pages
- [ ] Privacy policy (Datenschutzerklärung) accessible
- [ ] Cookie consent banner displays correctly
- [ ] German date format (DD.MM.YYYY) displays correctly
- [ ] Currency format (1.234,56 €) displays correctly
- [ ] VAT calculation is correct (19%)
- [ ] Invoice contains all mandatory fields
- [ ] SEPA mandate generation works
- [ ] German postal code validation (5 digits)
- [ ] Phone number format with +49 country code
- [ ] German language translations complete
- [ ] Error messages in German
- [ ] Email templates in German
- [ ] Driving license validation for German licenses
- [ ] Age restriction (21+) enforced
- [ ] Terms & Conditions acceptance tracked

## Summary

### High Priority for MVP
1. ✅ GDPR compliance (anonymization, consent)
2. ✅ German language UI
3. ✅ VAT handling (19%)
4. ✅ German date/currency formats
5. ✅ Impressum & Datenschutzerklärung pages
6. ✅ Age validation (21+)
7. ✅ German postal code validation

### Medium Priority
1. ✅ SEPA payment method (IBAN/BIC validation, mandate management)
2. ✅ Invoice generation with legal requirements (§14 UStG compliant)
3. ✅ Driving license validation (21+ age, 1 year holding period, EU recognition)
4. ✅ Kilometer packages (100/200 km per day, unlimited, overage charging)
5. ✅ Vehicle extras (12 German market extras with pricing)
6. ✅ Insurance packages (Basic, Standard, Comfort, Premium with deductibles)
7. ⚠️ Cookie consent banner

### Future Enhancements
1. ✅ B2B customer support with VAT ID (CustomerType, VATId, CompanyName, PaymentTerms)
2. ⏳ Cross-border travel policies
3. ⏳ Framework agreements for corporate customers
4. ⏳ Multi-language support (English, French)

## Resources

- [DSGVO (GDPR)](https://dsgvo-gesetz.de/)
- [Impressumspflicht](https://www.e-recht24.de/impressum-generator.html)
- [German VAT Rates](https://www.bundeszentralamt-steuern.de/)
- [SEPA Direct Debit](https://www.europeanpaymentscouncil.eu/what-we-do/sepa-direct-debit)
- [BITV 2.0 Accessibility](https://www.gesetze-im-internet.de/bitv_2_0/)

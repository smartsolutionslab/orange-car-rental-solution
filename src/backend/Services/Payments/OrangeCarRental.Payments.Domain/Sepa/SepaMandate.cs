using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

/// <summary>
///     SEPA Direct Debit Mandate (SEPA-Lastschriftmandat) aggregate.
///     Required for SEPA direct debit payments in Germany.
/// </summary>
public sealed class SepaMandate : AggregateRoot<SepaMandateIdentifier>
{
    /// <summary>
    ///     Orange Car Rental's Creditor Identifier (Gläubiger-Identifikationsnummer).
    ///     This is a unique identifier assigned by the Bundesbank.
    /// </summary>
    public const string CreditorId = "DE98ZZZ09999999999";

    /// <summary>
    ///     Creditor company name.
    /// </summary>
    public const string CreditorName = "Orange Car Rental GmbH";

    private SepaMandate()
    {
        MandateReference = null!;
        IBAN = null!;
        BIC = null!;
        AccountHolder = default;
    }

    /// <summary>
    ///     Unique mandate reference.
    /// </summary>
    public MandateReference MandateReference { get; init; }

    /// <summary>
    ///     Bank account IBAN.
    /// </summary>
    public IBAN IBAN { get; init; }

    /// <summary>
    ///     Bank identifier code.
    /// </summary>
    public BIC BIC { get; init; }

    /// <summary>
    ///     Account holder name.
    /// </summary>
    public PersonName AccountHolder { get; init; }

    /// <summary>
    ///     Mandate status.
    /// </summary>
    public MandateStatus Status { get; init; }

    /// <summary>
    ///     Referenced customer ID.
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    ///     Date when mandate was signed.
    /// </summary>
    public DateTime SignedAt { get; init; }

    /// <summary>
    ///     Date when mandate was revoked.
    /// </summary>
    public DateTime? RevokedAt { get; init; }

    /// <summary>
    ///     Date of last usage.
    /// </summary>
    public DateTime? LastUsedAt { get; init; }

    /// <summary>
    ///     Created timestamp.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Creates a new SEPA mandate.
    /// </summary>
    public static SepaMandate Create(
        MandateReference mandateReference,
        Guid customerId,
        IBAN iban,
        BIC bic,
        string accountHolder,
        DateTime signedAt)
    {
        return new SepaMandate
        {
            Id = SepaMandateIdentifier.New(),
            MandateReference = mandateReference,
            CustomerId = customerId,
            IBAN = iban,
            BIC = bic,
            AccountHolder = PersonName.Of(accountHolder),
            Status = MandateStatus.Active,
            SignedAt = signedAt,
            CreatedAt = DateTime.UtcNow
        };
    }

    private SepaMandate CreateMutatedCopy(
        MandateStatus? status = null,
        DateTime? revokedAt = null,
        DateTime? lastUsedAt = null)
    {
        return new SepaMandate
        {
            Id = Id,
            MandateReference = MandateReference,
            CustomerId = CustomerId,
            IBAN = IBAN,
            BIC = BIC,
            AccountHolder = AccountHolder,
            Status = status ?? Status,
            SignedAt = SignedAt,
            RevokedAt = revokedAt ?? RevokedAt,
            LastUsedAt = lastUsedAt ?? LastUsedAt,
            CreatedAt = CreatedAt
        };
    }

    /// <summary>
    ///     Records that the mandate was used for a payment.
    /// </summary>
    public SepaMandate RecordUsage()
    {
        if (Status != MandateStatus.Active)
            throw new InvalidOperationException($"Cannot use mandate in status: {Status}");

        return CreateMutatedCopy(lastUsedAt: DateTime.UtcNow);
    }

    /// <summary>
    ///     Revokes the mandate.
    /// </summary>
    public SepaMandate Revoke()
    {
        if (Status != MandateStatus.Active)
            throw new InvalidOperationException($"Cannot revoke mandate in status: {Status}");

        return CreateMutatedCopy(
            status: MandateStatus.Revoked,
            revokedAt: DateTime.UtcNow);
    }

    /// <summary>
    ///     Marks the mandate as expired.
    ///     A mandate expires if not used for 36 months.
    /// </summary>
    public SepaMandate MarkAsExpired()
    {
        if (Status != MandateStatus.Active)
            throw new InvalidOperationException($"Cannot expire mandate in status: {Status}");

        return CreateMutatedCopy(status: MandateStatus.Expired);
    }

    /// <summary>
    ///     Checks if the mandate has expired due to inactivity.
    ///     SEPA mandates expire after 36 months of no use.
    /// </summary>
    public bool IsExpiredDueToInactivity()
    {
        if (Status != MandateStatus.Active)
            return false;

        var lastActivity = LastUsedAt ?? SignedAt;
        var monthsSinceLastUse = (DateTime.UtcNow - lastActivity).TotalDays / 30;

        return monthsSinceLastUse >= 36;
    }

    /// <summary>
    ///     Gets the masked IBAN for display (e.g., DE89 **** **** **** **** 00).
    /// </summary>
    public string GetMaskedIBAN()
    {
        var iban = IBAN.Value;
        if (iban.Length < 8)
            return iban;

        // Show first 4 and last 2 characters
        return $"{iban[..4]} **** **** **** **** {iban[^2..]}";
    }
}

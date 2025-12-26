using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Tests.Sepa;

public class SepaMandateTests
{
    private readonly MandateReference _validReference = MandateReference.Create(1, TestDates.Today);
    private readonly IBAN _validIBAN = IBAN.Create("DE89370400440532013000");
    private readonly BIC _validBIC = BIC.Create("DEUTDEDBFRA");
    private readonly CustomerId _customerId = CustomerId.From(TestIds.Customer1);
    private static string ValidAccountHolder => TestCustomer.MaxMustermann.FullName;

    [Fact]
    public void Create_ValidData_CreatesSepaMandate()
    {
        // Arrange
        var signedAt = DateTime.UtcNow;

        // Act
        var mandate = SepaMandate.Create(
            _validReference,
            _customerId,
            _validIBAN,
            _validBIC,
            ValidAccountHolder,
            signedAt);

        // Assert
        mandate.Id.Value.ShouldNotBe(Guid.Empty);
        mandate.MandateReference.ShouldBe(_validReference);
        mandate.CustomerId.Value.ShouldBe(_customerId.Value);
        mandate.IBAN.ShouldBe(_validIBAN);
        mandate.BIC.ShouldBe(_validBIC);
        mandate.AccountHolder.Value.ShouldBe(ValidAccountHolder);
        mandate.Status.ShouldBe(MandateStatus.Active);
        mandate.SignedAt.ShouldBe(signedAt);
        mandate.RevokedAt.ShouldBeNull();
        mandate.LastUsedAt.ShouldBeNull();
    }

    [Fact]
    public void Create_EmptyAccountHolder_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => SepaMandate.Create(
            _validReference,
            _customerId,
            _validIBAN,
            _validBIC,
            "",
            DateTime.UtcNow));
    }

    [Fact]
    public void Create_WhitespaceAccountHolder_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => SepaMandate.Create(
            _validReference,
            _customerId,
            _validIBAN,
            _validBIC,
            "   ",
            DateTime.UtcNow));
    }

    [Fact]
    public void CreditorId_HasCorrectGermanFormat()
    {
        // Assert - German Creditor ID format: DE + 2 check + ZZZ + 8 unique
        SepaMandate.CreditorId.ShouldStartWith("DE");
        SepaMandate.CreditorId.Length.ShouldBe(18);
    }

    [Fact]
    public void RecordUsage_ActiveMandate_UpdatesLastUsedAt()
    {
        // Arrange
        var mandate = CreateActiveMandate();
        var beforeUsage = DateTime.UtcNow;

        // Act
        var updatedMandate = mandate.RecordUsage();

        // Assert
        updatedMandate.LastUsedAt.ShouldNotBeNull();
        updatedMandate.LastUsedAt!.Value.ShouldBeGreaterThanOrEqualTo(beforeUsage);
        updatedMandate.Status.ShouldBe(MandateStatus.Active);

        // Original mandate unchanged (immutability)
        mandate.LastUsedAt.ShouldBeNull();
    }

    [Fact]
    public void RecordUsage_RevokedMandate_ThrowsInvalidOperationException()
    {
        // Arrange
        var mandate = CreateActiveMandate().Revoke();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => mandate.RecordUsage());
    }

    [Fact]
    public void RecordUsage_ExpiredMandate_ThrowsInvalidOperationException()
    {
        // Arrange
        var mandate = CreateActiveMandate().MarkAsExpired();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => mandate.RecordUsage());
    }

    [Fact]
    public void Revoke_ActiveMandate_UpdatesStatusAndRevokedAt()
    {
        // Arrange
        var mandate = CreateActiveMandate();
        var beforeRevoke = DateTime.UtcNow;

        // Act
        var revokedMandate = mandate.Revoke();

        // Assert
        revokedMandate.Status.ShouldBe(MandateStatus.Revoked);
        revokedMandate.RevokedAt.ShouldNotBeNull();
        revokedMandate.RevokedAt!.Value.ShouldBeGreaterThanOrEqualTo(beforeRevoke);

        // Original mandate unchanged (immutability)
        mandate.Status.ShouldBe(MandateStatus.Active);
    }

    [Fact]
    public void Revoke_RevokedMandate_ThrowsInvalidOperationException()
    {
        // Arrange
        var mandate = CreateActiveMandate().Revoke();

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() => mandate.Revoke());
        ex.Message.ShouldContain("Revoked");
    }

    [Fact]
    public void Revoke_ExpiredMandate_ThrowsInvalidOperationException()
    {
        // Arrange
        var mandate = CreateActiveMandate().MarkAsExpired();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => mandate.Revoke());
    }

    [Fact]
    public void MarkAsExpired_ActiveMandate_UpdatesStatus()
    {
        // Arrange
        var mandate = CreateActiveMandate();

        // Act
        var expiredMandate = mandate.MarkAsExpired();

        // Assert
        expiredMandate.Status.ShouldBe(MandateStatus.Expired);

        // Original mandate unchanged (immutability)
        mandate.Status.ShouldBe(MandateStatus.Active);
    }

    [Fact]
    public void MarkAsExpired_RevokedMandate_ThrowsInvalidOperationException()
    {
        // Arrange
        var mandate = CreateActiveMandate().Revoke();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => mandate.MarkAsExpired());
    }

    [Fact]
    public void MarkAsExpired_AlreadyExpiredMandate_ThrowsInvalidOperationException()
    {
        // Arrange
        var mandate = CreateActiveMandate().MarkAsExpired();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => mandate.MarkAsExpired());
    }

    [Fact]
    public void IsExpiredDueToInactivity_ActiveMandateUsedRecently_ReturnsFalse()
    {
        // Arrange
        var mandate = CreateActiveMandate().RecordUsage();

        // Act & Assert
        mandate.IsExpiredDueToInactivity().ShouldBeFalse();
    }

    [Fact]
    public void IsExpiredDueToInactivity_RevokedMandate_ReturnsFalse()
    {
        // Arrange
        var mandate = CreateActiveMandate().Revoke();

        // Act & Assert
        mandate.IsExpiredDueToInactivity().ShouldBeFalse();
    }

    [Fact]
    public void IsExpiredDueToInactivity_ExpiredMandate_ReturnsFalse()
    {
        // Arrange
        var mandate = CreateActiveMandate().MarkAsExpired();

        // Act & Assert
        mandate.IsExpiredDueToInactivity().ShouldBeFalse();
    }

    [Fact]
    public void GetMaskedIBAN_ReturnsPartiallyMaskedIBAN()
    {
        // Arrange
        var mandate = CreateActiveMandate();

        // Act
        var maskedIBAN = mandate.GetMaskedIBAN();

        // Assert
        maskedIBAN.ShouldStartWith("DE89");
        maskedIBAN.ShouldEndWith("00");
        maskedIBAN.ShouldContain("****");
    }

    [Fact]
    public void Immutability_CreateMutatedCopy_PreservesOriginalValues()
    {
        // Arrange
        var mandate = CreateActiveMandate();
        var originalId = mandate.Id;
        var originalReference = mandate.MandateReference;
        var originalIBAN = mandate.IBAN;

        // Act
        var usedMandate = mandate.RecordUsage();

        // Assert
        mandate.Id.ShouldBe(originalId);
        mandate.MandateReference.ShouldBe(originalReference);
        mandate.IBAN.ShouldBe(originalIBAN);
        mandate.LastUsedAt.ShouldBeNull();

        usedMandate.Id.ShouldBe(originalId);
        usedMandate.MandateReference.ShouldBe(originalReference);
        usedMandate.IBAN.ShouldBe(originalIBAN);
        usedMandate.LastUsedAt.ShouldNotBeNull();
    }

    [Fact]
    public void StatusTransitions_FullWorkflow_TransitionsCorrectly()
    {
        // Arrange
        var mandate = CreateActiveMandate();

        // Assert initial state
        mandate.Status.ShouldBe(MandateStatus.Active);

        // Record usage
        var usedMandate = mandate.RecordUsage();
        usedMandate.Status.ShouldBe(MandateStatus.Active);
        usedMandate.LastUsedAt.ShouldNotBeNull();

        // Revoke
        var revokedMandate = usedMandate.Revoke();
        revokedMandate.Status.ShouldBe(MandateStatus.Revoked);
        revokedMandate.RevokedAt.ShouldNotBeNull();
    }

    private SepaMandate CreateActiveMandate()
    {
        return SepaMandate.Create(
            _validReference,
            _customerId,
            _validIBAN,
            _validBIC,
            ValidAccountHolder,
            DateTime.UtcNow);
    }
}

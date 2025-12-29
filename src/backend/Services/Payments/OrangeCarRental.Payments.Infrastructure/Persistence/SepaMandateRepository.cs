using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;
using SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Configuration;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence;

public sealed class SepaMandateRepository(
    PaymentsDbContext context,
    IOptions<SepaConfiguration> sepaOptions) : ISepaMandateRepository
{
    private readonly SepaConfiguration sepaConfig = sepaOptions.Value;

    public async Task<SepaMandate> GetByIdAsync(SepaMandateIdentifier id, CancellationToken cancellationToken = default)
    {
        var mandate = await context.SepaMandates
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        return mandate ?? throw new EntityNotFoundException(typeof(SepaMandate), id);
    }

    public async Task<SepaMandate?> GetActiveByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        return await context.SepaMandates
            .FirstOrDefaultAsync(m => m.CustomerId == customerId && m.Status == MandateStatus.Active, cancellationToken);
    }

    public async Task<IReadOnlyList<SepaMandate>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        return await context.SepaMandates
            .Where(m => m.CustomerId == customerId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<SepaMandate?> GetByMandateReferenceAsync(MandateReference reference, CancellationToken cancellationToken = default)
    {
        return await context.SepaMandates
            .FirstOrDefaultAsync(m => m.MandateReference == reference, cancellationToken);
    }

    public async Task<int> GetNextSequenceNumberAsync(CancellationToken cancellationToken = default)
    {
        var maxSequence = await context.SepaMandates
            .Select(m => m.MandateReference.Value)
            .ToListAsync(cancellationToken);

        if (maxSequence.Count == 0)
            return 1;

        // Extract sequence number from mandate reference (last 6 digits)
        var maxNumber = maxSequence
            .Select(r => int.TryParse(r[^6..], out var seq) ? seq : 0)
            .DefaultIfEmpty(0)
            .Max();

        return maxNumber + 1;
    }

    public async Task<IReadOnlyList<SepaMandate>> GetExpiredMandatesAsync(CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-sepaConfig.MandateExpiryMonths);

        return await context.SepaMandates
            .Where(m => m.Status == MandateStatus.Active)
            .Where(m => (m.LastUsedAt ?? m.SignedAt) < cutoffDate)
            .ToListAsync(cancellationToken);
    }

    public IAsyncEnumerable<SepaMandate> StreamExpiredMandatesAsync(CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddMonths(-sepaConfig.MandateExpiryMonths);

        return context.SepaMandates
            .Where(m => m.Status == MandateStatus.Active)
            .Where(m => (m.LastUsedAt ?? m.SignedAt) < cutoffDate)
            .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<SepaMandate> StreamByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        return context.SepaMandates
            .Where(m => m.CustomerId == customerId)
            .OrderByDescending(m => m.CreatedAt)
            .AsAsyncEnumerable();
    }

    public async Task AddAsync(SepaMandate mandate, CancellationToken cancellationToken = default) =>
        await context.SepaMandates.AddAsync(mandate, cancellationToken);

    public Task UpdateAsync(SepaMandate mandate, CancellationToken cancellationToken = default)
    {
        context.SepaMandates.Update(mandate);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(SepaMandate mandate, CancellationToken cancellationToken = default)
    {
        context.SepaMandates.Remove(mandate);
        return Task.CompletedTask;
    }
}

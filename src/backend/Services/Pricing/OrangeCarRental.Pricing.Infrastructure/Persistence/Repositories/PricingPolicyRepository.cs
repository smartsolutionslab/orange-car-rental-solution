using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence.Repositories;

/// <summary>
///     Entity Framework implementation of IPricingPolicyRepository.
/// </summary>
public sealed class PricingPolicyRepository(PricingDbContext context) : IPricingPolicyRepository
{
    public async Task<PricingPolicy> GetByIdAsync(PricingPolicyIdentifier id,
        CancellationToken cancellationToken = default)
    {
        var policy = await context.PricingPolicies
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return policy ?? throw new EntityNotFoundException(typeof(PricingPolicy), id);
    }

    public async Task<PricingPolicy> GetActivePolicyByCategoryAsync(
        CategoryCode categoryCode,
        CancellationToken cancellationToken = default)
    {
        var policy = await context.PricingPolicies
            .Where(p => p.CategoryCode == categoryCode && p.IsActive)
            .Where(p => p.LocationCode == null) // General pricing (not location-specific)
            .FirstOrDefaultAsync(cancellationToken);

        return policy ?? throw new EntityNotFoundException(
            typeof(PricingPolicy),
            categoryCode,
            $"No active pricing policy found for category '{categoryCode.Value}'");
    }

    public async Task<PricingPolicy> GetActivePolicyByCategoryAndLocationAsync(
        CategoryCode categoryCode,
        LocationCode locationCode,
        CancellationToken cancellationToken = default)
    {
        var policy = await context.PricingPolicies
            .Where(p => p.CategoryCode == categoryCode && p.IsActive)
            .Where(p => p.LocationCode == locationCode)
            .FirstOrDefaultAsync(cancellationToken);

        return policy ?? throw new EntityNotFoundException(
            typeof(PricingPolicy),
            new { categoryCode, locationCode },
            $"No active pricing policy found for category '{categoryCode.Value}' at location '{locationCode.Value}'");
    }

    public async Task<IReadOnlyCollection<PricingPolicy>> GetAllActivePoliciesAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.PricingPolicies
            .Where(p => p.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(PricingPolicy policy, CancellationToken cancellationToken = default) =>
        await context.PricingPolicies.AddAsync(policy, cancellationToken);

    public Task UpdateAsync(PricingPolicy policy, CancellationToken cancellationToken = default)
    {
        context.PricingPolicies.Update(policy);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}

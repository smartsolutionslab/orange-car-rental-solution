using SmartSolutionsLab.OrangeCarRental.Pricing.Domain;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;
using SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence.Repositories;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence;

/// <summary>
///     Unit of Work implementation for the Pricing bounded context.
///     Provides access to repositories and manages transactional boundaries.
/// </summary>
public sealed class PricingUnitOfWork : IPricingUnitOfWork
{
    private readonly PricingDbContext _context;
    private IPricingPolicyRepository? _pricingPolicies;

    public PricingUnitOfWork(PricingDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IPricingPolicyRepository PricingPolicies =>
        _pricingPolicies ??= new PricingPolicyRepository(_context);

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}

using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Persistence;

public sealed class PaymentRepository(PaymentsDbContext context) : IPaymentRepository
{
    public async Task<Payment> GetByIdAsync(PaymentIdentifier id, CancellationToken cancellationToken = default)
    {
        var payment = await context.Payments
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return payment ?? throw new EntityNotFoundException(typeof(Payment), id);
    }

    public async Task<Payment?> GetByReservationIdentifierAsync(ReservationIdentifier reservationId, CancellationToken cancellationToken = default)
    {
        return await context.Payments
            .FirstOrDefaultAsync(p => p.ReservationIdentifier == reservationId, cancellationToken);
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default) =>
        await context.Payments.AddAsync(payment, cancellationToken);

    public Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        context.Payments.Update(payment);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        context.Payments.Remove(payment);
        return Task.CompletedTask;
    }
}

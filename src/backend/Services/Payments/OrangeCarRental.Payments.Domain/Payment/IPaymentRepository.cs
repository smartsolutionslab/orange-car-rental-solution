using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

public interface IPaymentRepository : IRepository<Payment, PaymentIdentifier>
{
    Task<Payment?> GetByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default);
}

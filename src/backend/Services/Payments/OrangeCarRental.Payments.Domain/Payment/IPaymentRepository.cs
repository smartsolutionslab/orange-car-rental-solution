using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

public interface IPaymentRepository : IRepository<Payment, PaymentIdentifier>
{
    Task<Payment?> GetByReservationIdentifierAsync(ReservationIdentifier reservationId, CancellationToken cancellationToken = default);
}

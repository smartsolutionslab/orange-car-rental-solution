using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Queries.GetVehicleAvailability;

/// <summary>
///     Handler for GetVehicleAvailabilityQuery.
///     Returns list of vehicle IDs that are booked during the requested period.
/// </summary>
public sealed class GetVehicleAvailabilityQueryHandler(IReservationRepository reservations)
    : IQueryHandler<GetVehicleAvailabilityQuery, GetVehicleAvailabilityResult>
{
    public async Task<GetVehicleAvailabilityResult> HandleAsync(
        GetVehicleAvailabilityQuery query,
        CancellationToken cancellationToken = default)
    {
        var bookedVehicleIds = await reservations.GetBookedVehicleIdsAsync(
            query.PickupDate,
            query.ReturnDate,
            cancellationToken);

        return new GetVehicleAvailabilityResult(
            bookedVehicleIds,
            query.PickupDate,
            query.ReturnDate);
    }
}

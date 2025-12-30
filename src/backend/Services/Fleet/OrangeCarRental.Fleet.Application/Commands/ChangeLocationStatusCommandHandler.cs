using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;

/// <summary>
///     Handler for ChangeLocationStatusCommand.
///     Changes the status of a location.
/// </summary>
public sealed class ChangeLocationStatusCommandHandler(IFleetUnitOfWork unitOfWork)
    : ICommandHandler<ChangeLocationStatusCommand, ChangeLocationStatusResult>
{
    private ILocationRepository Locations => unitOfWork.Locations;

    /// <summary>
    ///     Handles the change location status command.
    /// </summary>
    /// <param name="command">The command with location code and new status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with old and new status.</returns>
    public async Task<ChangeLocationStatusResult> HandleAsync(
        ChangeLocationStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var (code, newStatus, reason) = command;

        var location = await Locations.GetByCodeAsync(code, cancellationToken);
        var oldStatus = location.Status;

        location = location.ChangeStatus(newStatus, reason);
        await Locations.UpdateAsync(location, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangeLocationStatusResult(
            location.Code.Value,
            oldStatus.ToString(),
            newStatus.ToString(),
            $"Location status changed from {oldStatus} to {newStatus}"
        );
    }
}

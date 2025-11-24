using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.ChangeLocationStatus;

/// <summary>
///     Handler for ChangeLocationStatusCommand.
///     Changes the status of a location.
/// </summary>
public sealed class ChangeLocationStatusCommandHandler(ILocationRepository locations)
    : ICommandHandler<ChangeLocationStatusCommand, ChangeLocationStatusResult>
{
    /// <summary>
    ///     Handles the change location status command.
    /// </summary>
    /// <param name="command">The command with location ID and new status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with old and new status.</returns>
    public async Task<ChangeLocationStatusResult> HandleAsync(
        ChangeLocationStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var (locationId, newStatus, reason) = command;

        var location = await locations.GetByIdAsync(locationId, cancellationToken);
        var oldStatus = location.Status;

        location = location.ChangeStatus(newStatus, reason);

        await locations.UpdateAsync(location, cancellationToken);
        await locations.SaveChangesAsync(cancellationToken);

        return new ChangeLocationStatusResult(
            location.Id.Value,
            location.Code.Value,
            oldStatus.ToString(),
            newStatus.ToString(),
            $"Location status changed from {oldStatus} to {newStatus}"
        );
    }
}

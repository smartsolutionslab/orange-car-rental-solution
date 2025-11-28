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
    /// <param name="command">The command with location code and new status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with old and new status.</returns>
    public async Task<ChangeLocationStatusResult> HandleAsync(
        ChangeLocationStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var (code, newStatus, reason) = command;

        var location = await locations.GetByCodeAsync(code, cancellationToken);
        var oldStatus = location.Status;

        var statusChangeReason = StatusChangeReason.FromNullable(reason);
        location = location.ChangeStatus(newStatus, statusChangeReason);

        await locations.UpdateAsync(location, cancellationToken);
        await locations.SaveChangesAsync(cancellationToken);

        return new ChangeLocationStatusResult(
            location.Code.Value,
            oldStatus.ToString(),
            newStatus.ToString(),
            $"Location status changed from {oldStatus} to {newStatus}"
        );
    }
}

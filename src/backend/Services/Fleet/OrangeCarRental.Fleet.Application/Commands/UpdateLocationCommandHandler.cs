using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;

/// <summary>
///     Handler for UpdateLocationCommand.
///     Updates location information.
/// </summary>
public sealed class UpdateLocationCommandHandler(IFleetUnitOfWork unitOfWork)
    : ICommandHandler<UpdateLocationCommand, UpdateLocationResult>
{
    private ILocationRepository Locations => unitOfWork.Locations;

    /// <summary>
    ///     Handles the update location command.
    /// </summary>
    /// <param name="command">The command with location code and new information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with updated location details.</returns>
    public async Task<UpdateLocationResult> HandleAsync(
        UpdateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var (code, name, address) = command;

        var location = await Locations.GetByCodeAsync(code, cancellationToken);
        location = location.UpdateInformation(name, address);

        await Locations.UpdateAsync(location, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateLocationResult(
            location.Code.Value,
            location.Name.Value,
            location.Address.FullAddress,
            "Location information updated successfully"
        );
    }
}

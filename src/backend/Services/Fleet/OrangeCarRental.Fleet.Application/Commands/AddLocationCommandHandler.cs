using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;

/// <summary>
///     Handler for AddLocationCommand.
///     Creates a new rental location.
/// </summary>
public sealed class AddLocationCommandHandler(IFleetUnitOfWork unitOfWork)
    : ICommandHandler<AddLocationCommand, AddLocationResult>
{
    private ILocationRepository Locations => unitOfWork.Locations;

    /// <summary>
    ///     Handles the add location command.
    /// </summary>
    /// <param name="command">The command with location data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with the new location code and details.</returns>
    public async Task<AddLocationResult> HandleAsync(
        AddLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var (code, name, address) = command;

        await EnsureLocationCodeNotExists(code, cancellationToken);

        var location = Location.Create(code, name, address);
        await Locations.AddAsync(location, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddLocationResult(
            location.Code.Value,
            location.Name.Value,
            location.Address.FullAddress,
            location.Status.ToString()
        );
    }

    private async Task EnsureLocationCodeNotExists(LocationCode code, CancellationToken cancellationToken)
    {
        var existingLocation = await Locations.FindByCodeAsync(code, cancellationToken);
        if (existingLocation != null)
        {
            throw new InvalidOperationException($"A location with code '{code.Value}' already exists.");
        }
    }
}

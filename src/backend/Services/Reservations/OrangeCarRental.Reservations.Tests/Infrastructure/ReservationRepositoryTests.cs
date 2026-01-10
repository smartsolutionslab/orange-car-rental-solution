using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using Shouldly;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;
using SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Tests.Infrastructure;

public class ReservationRepositoryTests : IAsyncLifetime
{
    // Configure SQL Server container (requires Docker to be running)
    private readonly MsSqlContainer msSqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private ReservationsDbContext context = null!;
    private ReservationRepository repository = null!;

    public async Task InitializeAsync()
    {
        // Start the SQL Server container
        await msSqlContainer.StartAsync();

        // Create DbContext with SQL Server connection
        var options = new DbContextOptionsBuilder<ReservationsDbContext>()
            .UseSqlServer(msSqlContainer.GetConnectionString())
            .Options;

        context = new ReservationsDbContext(options);
        await context.Database.EnsureCreatedAsync();
        repository = new ReservationRepository(context);
    }

    public async Task DisposeAsync()
    {
        await context.DisposeAsync();
        await msSqlContainer.DisposeAsync();
    }

    private Reservation CreateTestReservation()
    {
        var vehicleId = VehicleIdentifier.New();
        var customerId = CustomerIdentifier.New();
        var pickupDate = DateTime.UtcNow.Date.AddDays(7);
        var returnDate = pickupDate.AddDays(3);
        var period = BookingPeriod.Of(pickupDate, returnDate);
        var currency = Currency.EUR;
        var totalPrice = Money.FromGross(200.00m, 0.19m, currency);

        return Reservation.Create(
            vehicleId,
            customerId,
            period,
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            totalPrice);
    }

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsReservation()
    {
        // Arrange
        var reservation = CreateTestReservation();
        await context.Reservations.AddAsync(reservation);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(reservation.Id);
        result.VehicleIdentifier.ShouldBe(reservation.VehicleIdentifier);
        result.CustomerIdentifier.ShouldBe(reservation.CustomerIdentifier);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var nonExistingId = ReservationIdentifier.New();

        // Act & Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
            await repository.GetByIdAsync(nonExistingId, CancellationToken.None));
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WithNoReservations_ReturnsEmptyList()
    {
        // Act
        var result = await repository.GetAllAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleReservations_ReturnsAll()
    {
        // Arrange
        var reservations = new[] { CreateTestReservation(), CreateTestReservation(), CreateTestReservation() };

        await context.Reservations.AddRangeAsync(reservations);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_WithValidReservation_AddsToContext()
    {
        // Arrange
        var reservation = CreateTestReservation();

        // Act
        await repository.AddAsync(reservation, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var saved = await context.Reservations.FindAsync(reservation.Id);
        saved.ShouldNotBeNull();
        saved!.Id.ShouldBe(reservation.Id);
    }

    [Fact]
    public async Task AddAsync_WithMultipleReservations_AddsAll()
    {
        // Arrange
        var reservation1 = CreateTestReservation();
        var reservation2 = CreateTestReservation();

        // Act
        await repository.AddAsync(reservation1, CancellationToken.None);
        await repository.AddAsync(reservation2, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var all = await context.Reservations.ToListAsync();
        all.Count.ShouldBe(2);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithExistingReservation_UpdatesProperties()
    {
        // Arrange
        var reservation = CreateTestReservation();
        await context.Reservations.AddAsync(reservation);
        await context.SaveChangesAsync();

        // Detach to simulate loading in new context
        context.Entry(reservation).State = EntityState.Detached;

        // Load fresh copy and confirm (immutable pattern returns new instance)
        var loaded = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(loaded!).State = EntityState.Detached;
        var confirmed = loaded!.Confirm();

        // Act
        await repository.UpdateAsync(confirmed, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var updated = await context.Reservations.FindAsync(confirmed.Id);
        updated.ShouldNotBeNull();
        updated!.Status.ShouldBe(ReservationStatus.Confirmed);
        updated.ConfirmedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithStatusChange_PersistsChange()
    {
        // Arrange
        var pickupDate = DateTime.UtcNow.Date;
        var returnDate = pickupDate.AddDays(3);
        var period = BookingPeriod.Of(pickupDate, returnDate);
        var currency = Currency.From("EUR");
        var totalPrice = Money.FromGross(200.00m, 0.19m, currency);

        var reservation = Reservation.Create(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            period,
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            totalPrice);
        await context.Reservations.AddAsync(reservation);
        await context.SaveChangesAsync();

        // Detach and reload
        context.Entry(reservation).State = EntityState.Detached;
        var loaded = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(loaded!).State = EntityState.Detached;

        // Confirm and activate (immutable pattern returns new instances)
        var active = loaded!.Confirm().MarkAsActive();

        // Act
        await repository.UpdateAsync(active, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var updated = await context.Reservations.FindAsync(active.Id);
        updated.ShouldNotBeNull();
        updated!.Status.ShouldBe(ReservationStatus.Active);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithExistingReservation_RemovesIt()
    {
        // Arrange
        var reservation = CreateTestReservation();
        await context.Reservations.AddAsync(reservation);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(reservation.Id, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var deleted = await context.Reservations.FindAsync(reservation.Id);
        deleted.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_DoesNotThrow()
    {
        // Arrange
        var nonExistingId = ReservationIdentifier.New();

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            await repository.DeleteAsync(nonExistingId, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        });
    }

    #endregion

    #region SaveChangesAsync Tests

    [Fact]
    public async Task SaveChangesAsync_AfterAdd_PersistsChanges()
    {
        // Arrange
        var reservation = CreateTestReservation();
        await repository.AddAsync(reservation, CancellationToken.None);

        // Act
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var saved = await context.Reservations.FindAsync(reservation.Id);
        saved.ShouldNotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_AfterUpdate_PersistsChanges()
    {
        // Arrange
        var reservation = CreateTestReservation();
        await context.Reservations.AddAsync(reservation);
        await context.SaveChangesAsync();

        context.Entry(reservation).State = EntityState.Detached;
        var loaded = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(loaded!).State = EntityState.Detached;
        var confirmed = loaded!.Confirm();
        await repository.UpdateAsync(confirmed, CancellationToken.None);

        // Act
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var updated = await context.Reservations.FindAsync(confirmed.Id);
        updated!.Status.ShouldBe(ReservationStatus.Confirmed);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task CompleteWorkflow_CreateConfirmActivateComplete_Works()
    {
        // Arrange - Create reservation for today
        var pickupDate = DateTime.UtcNow.Date;
        var returnDate = pickupDate.AddDays(3);
        var period = BookingPeriod.Of(pickupDate, returnDate);
        var currency = Currency.From("EUR");
        var totalPrice = Money.FromGross(300.00m, 0.19m, currency);

        var reservation = Reservation.Create(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            period,
            LocationCode.From("BER-HBF"),
            LocationCode.From("BER-HBF"),
            totalPrice);

        // Act - Add
        await repository.AddAsync(reservation, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert - Pending state
        var pending = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        pending.ShouldNotBeNull();
        pending!.Status.ShouldBe(ReservationStatus.Pending);

        // Act - Confirm (immutable pattern returns new instance)
        context.Entry(pending).State = EntityState.Detached;
        var toConfirm = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(toConfirm!).State = EntityState.Detached;
        var confirmedRes = toConfirm!.Confirm();
        await repository.UpdateAsync(confirmedRes, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert - Confirmed state
        var confirmed = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        confirmed!.Status.ShouldBe(ReservationStatus.Confirmed);

        // Act - Activate (immutable pattern returns new instance)
        context.Entry(confirmed).State = EntityState.Detached;
        var toActivate = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(toActivate!).State = EntityState.Detached;
        var activeRes = toActivate!.MarkAsActive();
        await repository.UpdateAsync(activeRes, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert - Active state
        var active = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        active!.Status.ShouldBe(ReservationStatus.Active);

        // Act - Complete (immutable pattern returns new instance)
        context.Entry(active).State = EntityState.Detached;
        var toComplete = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(toComplete!).State = EntityState.Detached;
        var completedRes = toComplete!.Complete();
        await repository.UpdateAsync(completedRes, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert - Completed state
        var completed = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        completed!.Status.ShouldBe(ReservationStatus.Completed);
        completed.CompletedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task CancellationWorkflow_CreateAndCancel_Works()
    {
        // Arrange
        var reservation = CreateTestReservation();

        // Act - Add
        await repository.AddAsync(reservation, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act - Cancel (immutable pattern returns new instance)
        context.Entry(reservation).State = EntityState.Detached;
        var toCancel = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(toCancel!).State = EntityState.Detached;
        var cancelledRes = toCancel!.Cancel("Customer changed plans");
        await repository.UpdateAsync(cancelledRes, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var cancelled = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        cancelled!.Status.ShouldBe(ReservationStatus.Cancelled);
        cancelled.CancellationReason.ShouldBe("Customer changed plans");
        cancelled.CancelledAt.ShouldNotBeNull();
    }

    #endregion
}

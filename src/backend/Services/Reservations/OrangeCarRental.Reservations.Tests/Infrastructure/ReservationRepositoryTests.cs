using Microsoft.EntityFrameworkCore;
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
    // Configure SQL Server container
    private readonly MsSqlContainer msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
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
        var currency = Currency.Of("EUR");
        var totalPrice = Money.FromGross(200.00m, 0.19m, currency);

        var reservation = Reservation.Create(
            vehicleId,
            customerId,
            period,
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
            totalPrice);
        reservation.ClearDomainEvents();
        return reservation;
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
        await repository.SaveChangesAsync(CancellationToken.None);

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
        await repository.SaveChangesAsync(CancellationToken.None);

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

        // Load fresh copy
        var loaded = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(loaded!).State = EntityState.Detached;
        loaded = loaded!.Confirm();

        // Act
        await repository.UpdateAsync(loaded, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        var updated = await context.Reservations.FindAsync(loaded.Id);
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
        var currency = Currency.Of("EUR");
        var totalPrice = Money.FromGross(200.00m, 0.19m, currency);

        var reservation = Reservation.Create(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            period,
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
            totalPrice);
        await context.Reservations.AddAsync(reservation);
        await context.SaveChangesAsync();

        // Detach and reload
        context.Entry(reservation).State = EntityState.Detached;
        var loaded = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(loaded!).State = EntityState.Detached;

        // Confirm and activate
        loaded = loaded!.Confirm();
        loaded = loaded.MarkAsActive();

        // Act
        await repository.UpdateAsync(loaded, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        var updated = await context.Reservations.FindAsync(loaded.Id);
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
        await repository.SaveChangesAsync(CancellationToken.None);

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
            await repository.SaveChangesAsync(CancellationToken.None);
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
        await repository.SaveChangesAsync(CancellationToken.None);

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
        loaded = loaded!.Confirm();
        await repository.UpdateAsync(loaded, CancellationToken.None);

        // Act
        await repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        var updated = await context.Reservations.FindAsync(loaded.Id);
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
        var currency = Currency.Of("EUR");
        var totalPrice = Money.FromGross(300.00m, 0.19m, currency);

        var reservation = Reservation.Create(
            VehicleIdentifier.New(),
            CustomerIdentifier.New(),
            period,
            LocationCode.Of("BER-HBF"),
            LocationCode.Of("BER-HBF"),
            totalPrice);

        // Act - Add
        await repository.AddAsync(reservation, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        // Assert - Pending state
        var pending = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        pending.ShouldNotBeNull();
        pending!.Status.ShouldBe(ReservationStatus.Pending);

        // Act - Confirm
        context.Entry(pending).State = EntityState.Detached;
        var toConfirm = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(toConfirm!).State = EntityState.Detached;
        toConfirm = toConfirm!.Confirm();
        await repository.UpdateAsync(toConfirm, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        // Assert - Confirmed state
        var confirmed = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        confirmed!.Status.ShouldBe(ReservationStatus.Confirmed);

        // Act - Activate
        context.Entry(confirmed).State = EntityState.Detached;
        var toActivate = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(toActivate!).State = EntityState.Detached;
        toActivate = toActivate!.MarkAsActive();
        await repository.UpdateAsync(toActivate, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        // Assert - Active state
        var active = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        active!.Status.ShouldBe(ReservationStatus.Active);

        // Act - Complete
        context.Entry(active).State = EntityState.Detached;
        var toComplete = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(toComplete!).State = EntityState.Detached;
        toComplete = toComplete!.Complete();
        await repository.UpdateAsync(toComplete, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

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
        await repository.SaveChangesAsync(CancellationToken.None);

        // Act - Cancel
        context.Entry(reservation).State = EntityState.Detached;
        var toCancel = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        context.Entry(toCancel!).State = EntityState.Detached;
        toCancel = toCancel!.Cancel("Customer changed plans");
        await repository.UpdateAsync(toCancel, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        var cancelled = await repository.GetByIdAsync(reservation.Id, CancellationToken.None);
        cancelled!.Status.ShouldBe(ReservationStatus.Cancelled);
        cancelled.CancellationReason.ShouldBe("Customer changed plans");
        cancelled.CancelledAt.ShouldNotBeNull();
    }

    #endregion
}

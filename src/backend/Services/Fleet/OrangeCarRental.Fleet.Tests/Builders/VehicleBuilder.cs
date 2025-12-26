using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Tests.Builders;

/// <summary>
/// Test data builder for Vehicle aggregates.
/// Uses fluent API with sensible defaults.
/// </summary>
public class VehicleBuilder
{
    private VehicleName _name = VehicleName.From("BMW X5");
    private VehicleCategory _category = VehicleCategory.SUV;
    private LocationCode _location = LocationCode.From(TestLocations.BerlinHbf);
    private Money _dailyRate = TestMoney.DailyRates.Suv;
    private SeatingCapacity _seats = SeatingCapacity.From(5);
    private FuelType _fuelType = FuelType.Diesel;
    private TransmissionType _transmission = TransmissionType.Automatic;
    private LicensePlate? _licensePlate;
    private Manufacturer? _manufacturer;
    private VehicleModel? _model;
    private ManufacturingYear? _year;
    private ImageUrl? _imageUrl;

    /// <summary>
    /// Sets the vehicle name.
    /// </summary>
    public VehicleBuilder WithName(string name)
    {
        _name = VehicleName.From(name);
        return this;
    }

    /// <summary>
    /// Sets the vehicle category.
    /// </summary>
    public VehicleBuilder WithCategory(VehicleCategory category)
    {
        _category = category;
        return this;
    }

    /// <summary>
    /// Sets the vehicle as a Compact car.
    /// </summary>
    public VehicleBuilder AsCompact()
    {
        _category = VehicleCategory.Kompaktklasse;
        _name = VehicleName.From("VW Golf");
        _seats = SeatingCapacity.From(5);
        _dailyRate = TestMoney.DailyRates.Kompakt;
        return this;
    }

    /// <summary>
    /// Sets the vehicle as a Mid-size car.
    /// </summary>
    public VehicleBuilder AsMidSize()
    {
        _category = VehicleCategory.Mittelklasse;
        _name = VehicleName.From("VW Passat");
        _seats = SeatingCapacity.From(5);
        _dailyRate = TestMoney.DailyRates.Mittel;
        return this;
    }

    /// <summary>
    /// Sets the vehicle as an SUV.
    /// </summary>
    public VehicleBuilder AsSuv()
    {
        _category = VehicleCategory.SUV;
        _name = VehicleName.From("BMW X5");
        _seats = SeatingCapacity.From(5);
        _dailyRate = TestMoney.DailyRates.Suv;
        return this;
    }

    /// <summary>
    /// Sets the vehicle as a Luxury car.
    /// </summary>
    public VehicleBuilder AsLuxury()
    {
        _category = VehicleCategory.Luxus;
        _name = VehicleName.From("Mercedes S-Class");
        _seats = SeatingCapacity.From(5);
        _dailyRate = TestMoney.DailyRates.Luxus;
        return this;
    }

    /// <summary>
    /// Sets the current location.
    /// </summary>
    public VehicleBuilder AtLocation(LocationCode location)
    {
        _location = location;
        return this;
    }

    /// <summary>
    /// Sets the daily rental rate.
    /// </summary>
    public VehicleBuilder WithDailyRate(decimal amount)
    {
        _dailyRate = Money.Euro(amount);
        return this;
    }

    /// <summary>
    /// Sets the seating capacity.
    /// </summary>
    public VehicleBuilder WithSeats(int seats)
    {
        _seats = SeatingCapacity.From(seats);
        return this;
    }

    /// <summary>
    /// Sets the fuel type.
    /// </summary>
    public VehicleBuilder WithFuelType(FuelType fuelType)
    {
        _fuelType = fuelType;
        return this;
    }

    /// <summary>
    /// Sets the vehicle as electric.
    /// </summary>
    public VehicleBuilder AsElectric()
    {
        _fuelType = FuelType.Electric;
        return this;
    }

    /// <summary>
    /// Sets the vehicle as hybrid.
    /// </summary>
    public VehicleBuilder AsHybrid()
    {
        _fuelType = FuelType.Hybrid;
        return this;
    }

    /// <summary>
    /// Sets the transmission type.
    /// </summary>
    public VehicleBuilder WithTransmission(TransmissionType transmission)
    {
        _transmission = transmission;
        return this;
    }

    /// <summary>
    /// Sets the vehicle as manual transmission.
    /// </summary>
    public VehicleBuilder AsManual()
    {
        _transmission = TransmissionType.Manual;
        return this;
    }

    /// <summary>
    /// Sets the license plate.
    /// </summary>
    public VehicleBuilder WithLicensePlate(string licensePlate)
    {
        _licensePlate = LicensePlate.From(licensePlate);
        return this;
    }

    /// <summary>
    /// Sets the manufacturer details.
    /// </summary>
    public VehicleBuilder WithDetails(string manufacturer, string model, int year, string? imageUrl = null)
    {
        _manufacturer = Manufacturer.From(manufacturer);
        _model = VehicleModel.From(model);
        _year = ManufacturingYear.From(year);
        _imageUrl = imageUrl != null ? ImageUrl.From(imageUrl) : null;
        return this;
    }

    /// <summary>
    /// Builds the vehicle in Available status.
    /// </summary>
    public Vehicle Build()
    {
        var vehicle = Vehicle.From(
            _name,
            _category,
            _location,
            _dailyRate,
            _seats,
            _fuelType,
            _transmission);

        if (_licensePlate.HasValue)
        {
            vehicle = vehicle.SetLicensePlate(_licensePlate.Value);
        }

        if (_manufacturer != null && _model != null && _year != null)
        {
            vehicle = vehicle.SetDetails(_manufacturer, _model, _year, _imageUrl);
        }

        return vehicle;
    }

    /// <summary>
    /// Builds the vehicle in Rented status.
    /// </summary>
    public Vehicle BuildRented()
    {
        var vehicle = Build();
        return vehicle.MarkAsRented();
    }

    /// <summary>
    /// Builds the vehicle in Maintenance status.
    /// </summary>
    public Vehicle BuildInMaintenance()
    {
        var vehicle = Build();
        return vehicle.MarkAsUnderMaintenance();
    }

    /// <summary>
    /// Builds the vehicle in Reserved status.
    /// </summary>
    public Vehicle BuildReserved()
    {
        var vehicle = Build();
        return vehicle.ChangeStatus(VehicleStatus.Reserved);
    }

    /// <summary>
    /// Creates a new VehicleBuilder with default values.
    /// </summary>
    public static VehicleBuilder Default() => new();

    /// <summary>
    /// Creates a BMW X5 SUV (default premium vehicle).
    /// </summary>
    public static VehicleBuilder BmwX5() => new VehicleBuilder()
        .AsSuv()
        .WithName("BMW X5")
        .WithDetails("BMW", "X5 xDrive40i", 2024)
        .AsElectric();

    /// <summary>
    /// Creates a VW Golf Compact car.
    /// </summary>
    public static VehicleBuilder VwGolf() => new VehicleBuilder()
        .AsCompact()
        .WithName("VW Golf")
        .WithDetails("Volkswagen", "Golf 8", 2023)
        .WithFuelType(FuelType.Petrol);

    /// <summary>
    /// Creates a Tesla Model 3 electric car.
    /// </summary>
    public static VehicleBuilder TeslaModel3() => new VehicleBuilder()
        .AsMidSize()
        .WithName("Tesla Model 3")
        .WithDetails("Tesla", "Model 3 Long Range", 2024)
        .AsElectric()
        .WithTransmission(TransmissionType.Automatic);
}

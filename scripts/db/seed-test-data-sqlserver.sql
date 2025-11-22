-- ============================================================================
-- Orange Car Rental - Test Data Seeding Script (SQL Server)
-- ============================================================================
-- Populates the database with realistic test data for development and E2E testing
-- Run after migrations have been applied
-- ============================================================================

USE OrangeCarRental_Customers;
GO

-- ============================================================================
-- CUSTOMERS
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM customers.Customers WHERE Email = 'max.mustermann@example.com')
BEGIN
    INSERT INTO customers.Customers (
        CustomerId, Salutation, FirstName, LastName, Email, PhoneNumber, DateOfBirth,
        DriversLicense_LicenseNumber, DriversLicense_ExpiryDate, DriversLicense_IssueDate, DriversLicense_IssueCountry,
        Address_Street, Address_City, Address_PostalCode, Address_Country,
        Status, RegisteredAtUtc, UpdatedAtUtc
    )
    VALUES
        (NEWID(), 'Mr', 'Max', 'Mustermann', 'max.mustermann@example.com', '+49 30 12345678', '1985-03-15',
         'D1234567890', '2030-03-15', '2020-03-15', 'Deutschland',
         'Hauptstraße 1', 'Berlin', '10115', 'Deutschland',
         'Active', GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Ms', 'Anna', 'Schmidt', 'anna.schmidt@example.com', '+49 89 98765432', '1990-07-22',
         'D0987654321', '2031-07-22', '2021-07-22', 'Deutschland',
         'Leopoldstraße 15', 'München', '80802', 'Deutschland',
         'Active', GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Mr', 'Thomas', 'Müller', 'thomas.mueller@example.com', '+49 40 55555555', '1988-11-30',
         'D1122334455', '2029-11-30', '2019-11-30', 'Deutschland',
         'Mönckebergstraße 5', 'Hamburg', '20095', 'Deutschland',
         'Active', GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Ms', 'Julia', 'Wagner', 'julia.wagner@example.com', '+49 221 66666666', '1992-05-18',
         'D5544332211', '2032-05-18', '2022-05-18', 'Deutschland',
         'Hohe Straße 10', 'Köln', '50667', 'Deutschland',
         'Active', GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Mr', 'Michael', 'Becker', 'michael.becker@example.com', '+49 711 77777777', '1987-09-25',
         'D9988776655', '2028-09-25', '2018-09-25', 'Deutschland',
         'Königstraße 20', 'Stuttgart', '70173', 'Deutschland',
         'Active', GETUTCDATE(), GETUTCDATE());

    PRINT 'Inserted 5 customers';
END
ELSE
    PRINT 'Customers already exist, skipping...';
GO

USE OrangeCarRental_Fleet;
GO

-- ============================================================================
-- VEHICLES
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM fleet.Vehicles WHERE LicensePlate = 'B-AB 1234')
BEGIN
    INSERT INTO fleet.Vehicles (
        VehicleId, Name, CategoryCode, Manufacturer, Model, Year, LicensePlate, LocationCode,
        Seats, FuelType, TransmissionType, Status, ImageUrl, Currency, DailyRateNet, DailyRateVat
    )
    VALUES
        (NEWID(), 'VW Golf Kompakt', 'COMPACT', 'VW', 'Golf', 2023, 'B-AB 1234', 'BER-HBF',
         5, 'Benzin', 'Manual', 0, 'https://example.com/golf.jpg', 'EUR', 37.82, 7.18),
        (NEWID(), 'Ford Focus Kompakt', 'COMPACT', 'Ford', 'Focus', 2022, 'M-CD 5678', 'MUC-APT',
         5, 'Benzin', 'Manual', 0, 'https://example.com/focus.jpg', 'EUR', 35.29, 6.71),
        (NEWID(), 'VW Tiguan SUV', 'SUV', 'VW', 'Tiguan', 2024, 'HH-EF 9012', 'HAM-CTR',
         5, 'Diesel', 'Automatic', 0, 'https://example.com/tiguan.jpg', 'EUR', 63.03, 11.97),
        (NEWID(), 'BMW 3er Mittelklasse', 'MIDSIZE', 'BMW', '3er', 2023, 'K-GH 3456', 'CGN-MSE',
         5, 'Diesel', 'Automatic', 1, 'https://example.com/bmw3.jpg', 'EUR', 71.43, 13.57),
        (NEWID(), 'Fiat 500 Mini', 'MINI', 'Fiat', '500', 2022, 'B-IJ 7890', 'BER-TXL',
         4, 'Benzin', 'Manual', 0, 'https://example.com/fiat500.jpg', 'EUR', 29.41, 5.59),
        (NEWID(), 'Mercedes S-Klasse Luxus', 'LUXURY', 'Mercedes', 'S-Klasse', 2024, 'F-KL 1234', 'FRA-HBF',
         5, 'Hybrid', 'Automatic', 0, 'https://example.com/mercedes-s.jpg', 'EUR', 126.05, 23.95),
        (NEWID(), 'VW Passat Variant Kombi', 'WAGON', 'VW', 'Passat Variant', 2023, 'S-MN 5678', 'STR-HBF',
         5, 'Diesel', 'Automatic', 0, 'https://example.com/passat.jpg', 'EUR', 54.62, 10.38),
        (NEWID(), 'BMW X3 SUV', 'SUV', 'BMW', 'X3', 2023, 'D-OP 9012', 'DUS-APT',
         5, 'Diesel', 'Automatic', 2, 'https://example.com/bmw-x3.jpg', 'EUR', 67.23, 12.77),
        (NEWID(), 'Opel Astra Kompakt', 'COMPACT', 'Opel', 'Astra', 2022, 'B-QR 3456', 'BER-HBF',
         5, 'Benzin', 'Manual', 0, 'https://example.com/astra.jpg', 'EUR', 33.61, 6.39),
        (NEWID(), 'Audi A4 Mittelklasse', 'MIDSIZE', 'Audi', 'A4', 2024, 'M-ST 7890', 'MUC-APT',
         5, 'Diesel', 'Automatic', 0, 'https://example.com/audi-a4.jpg', 'EUR', 63.03, 11.97);

    PRINT 'Inserted 10 vehicles';
END
ELSE
    PRINT 'Vehicles already exist, skipping...';
GO

USE OrangeCarRental_Location;
GO

-- ============================================================================
-- LOCATIONS
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM locations.Locations WHERE Code = 'BER-HBF')
BEGIN
    INSERT INTO locations.Locations (LocationId, Code, Name, Street, City, PostalCode, Country, Phone, Email, OpeningHours, Status, CreatedAt, UpdatedAt)
    VALUES
        (NEWID(), 'BER-HBF', 'Berlin Hauptbahnhof', 'Europaplatz 1', 'Berlin', '10557', 'Deutschland', '+49 30 11111111', 'berlin.hbf@orangecarrental.com', 'Mo-Su 06:00-22:00', 0, GETDATE(), GETDATE()),
        (NEWID(), 'MUC-APT', 'München Flughafen', 'Nordallee 25', 'München', '85356', 'Deutschland', '+49 89 22222222', 'muenchen.airport@orangecarrental.com', 'Mo-Su 05:00-23:00', 0, GETDATE(), GETDATE()),
        (NEWID(), 'HAM-CTR', 'Hamburg Zentrum', 'Mönckebergstraße 1', 'Hamburg', '20095', 'Deutschland', '+49 40 33333333', 'hamburg.zentrum@orangecarrental.com', 'Mo-Sa 08:00-20:00', 0, GETDATE(), GETDATE()),
        (NEWID(), 'CGN-MSE', 'Köln Messe', 'Messeplatz 1', 'Köln', '50679', 'Deutschland', '+49 221 44444444', 'koeln.messe@orangecarrental.com', 'Mo-Su 07:00-21:00', 0, GETDATE(), GETDATE()),
        (NEWID(), 'FRA-HBF', 'Frankfurt Hauptbahnhof', 'Am Hauptbahnhof', 'Frankfurt', '60329', 'Deutschland', '+49 69 55555555', 'frankfurt.hbf@orangecarrental.com', 'Mo-Su 06:00-22:00', 0, GETDATE(), GETDATE());

    PRINT 'Inserted 5 locations';
END
ELSE
    PRINT 'Locations already exist, skipping...';
GO

USE OrangeCarRental_Reservations;
GO

-- ============================================================================
-- RESERVATIONS
-- ============================================================================

DECLARE @CustomerId1 UNIQUEIDENTIFIER = NEWID();
DECLARE @CustomerId2 UNIQUEIDENTIFIER = NEWID();
DECLARE @CustomerId3 UNIQUEIDENTIFIER = NEWID();
DECLARE @CustomerId4 UNIQUEIDENTIFIER = NEWID();
DECLARE @CustomerId5 UNIQUEIDENTIFIER = NEWID();

DECLARE @VehicleId1 UNIQUEIDENTIFIER = NEWID();
DECLARE @VehicleId2 UNIQUEIDENTIFIER = NEWID();
DECLARE @VehicleId3 UNIQUEIDENTIFIER = NEWID();
DECLARE @VehicleId4 UNIQUEIDENTIFIER = NEWID();
DECLARE @VehicleId5 UNIQUEIDENTIFIER = NEWID();

IF NOT EXISTS (SELECT 1 FROM reservations.Reservations)
BEGIN
    -- Upcoming Reservations (Next week)
    INSERT INTO reservations.Reservations (
        ReservationId, CustomerId, VehicleId, PickupLocationCode, DropoffLocationCode,
        PickupDate, ReturnDate, Status, TotalPriceNet, TotalPriceVat, Currency, CreatedAt, ConfirmedAt
    )
    VALUES
        (
            NEWID(),
            @CustomerId1,
            @VehicleId1,
            'BER-HBF',
            'BER-HBF',
            CAST(DATEADD(DAY, 3, GETDATE()) AS DATE),
            CAST(DATEADD(DAY, 10, GETDATE()) AS DATE),
            'Confirmed',
            315.00,
            59.85,
            'EUR',
            DATEADD(DAY, -2, GETDATE()),
            DATEADD(DAY, -2, GETDATE())
        ),
        (
            NEWID(),
            @CustomerId2,
            @VehicleId2,
            'HAM-CTR',
            'HAM-CTR',
            CAST(DATEADD(DAY, 5, GETDATE()) AS DATE),
            CAST(DATEADD(DAY, 8, GETDATE()) AS DATE),
            'Confirmed',
            225.00,
            42.75,
            'EUR',
            DATEADD(DAY, -1, GETDATE()),
            DATEADD(DAY, -1, GETDATE())
        );

    -- Active Reservations (Currently rented)
    INSERT INTO reservations.Reservations (
        ReservationId, CustomerId, VehicleId, PickupLocationCode, DropoffLocationCode,
        PickupDate, ReturnDate, Status, TotalPriceNet, TotalPriceVat, Currency, CreatedAt, ConfirmedAt
    )
    VALUES
        (
            NEWID(),
            @CustomerId3,
            @VehicleId3,
            'CGN-MSE',
            'CGN-MSE',
            CAST(DATEADD(DAY, -2, GETDATE()) AS DATE),
            CAST(DATEADD(DAY, 5, GETDATE()) AS DATE),
            'Active',
            595.00,
            113.05,
            'EUR',
            DATEADD(DAY, -5, GETDATE()),
            DATEADD(DAY, -5, GETDATE())
        );

    -- Pending Reservations (Awaiting payment)
    INSERT INTO reservations.Reservations (
        ReservationId, CustomerId, VehicleId, PickupLocationCode, DropoffLocationCode,
        PickupDate, ReturnDate, Status, TotalPriceNet, TotalPriceVat, Currency, CreatedAt
    )
    VALUES
        (
            NEWID(),
            @CustomerId4,
            @VehicleId4,
            'FRA-HBF',
            'MUC-APT',
            CAST(DATEADD(DAY, 7, GETDATE()) AS DATE),
            CAST(DATEADD(DAY, 10, GETDATE()) AS DATE),
            'Pending',
            450.00,
            85.50,
            'EUR',
            GETDATE()
        );

    -- Completed Reservations (Past rentals)
    INSERT INTO reservations.Reservations (
        ReservationId, CustomerId, VehicleId, PickupLocationCode, DropoffLocationCode,
        PickupDate, ReturnDate, Status, TotalPriceNet, TotalPriceVat, Currency, CreatedAt, ConfirmedAt, CompletedAt
    )
    VALUES
        (
            NEWID(),
            @CustomerId1,
            @VehicleId5,
            'MUC-APT',
            'MUC-APT',
            CAST(DATEADD(DAY, -30, GETDATE()) AS DATE),
            CAST(DATEADD(DAY, -23, GETDATE()) AS DATE),
            'Completed',
            294.00,
            55.86,
            'EUR',
            DATEADD(DAY, -35, GETDATE()),
            DATEADD(DAY, -35, GETDATE()),
            DATEADD(DAY, -23, GETDATE())
        ),
        (
            NEWID(),
            @CustomerId2,
            @VehicleId1,
            'BER-TXL',
            'BER-HBF',
            CAST(DATEADD(DAY, -20, GETDATE()) AS DATE),
            CAST(DATEADD(DAY, -15, GETDATE()) AS DATE),
            'Completed',
            175.00,
            33.25,
            'EUR',
            DATEADD(DAY, -25, GETDATE()),
            DATEADD(DAY, -25, GETDATE()),
            DATEADD(DAY, -15, GETDATE())
        ),
        (
            NEWID(),
            @CustomerId5,
            @VehicleId2,
            'STR-HBF',
            'STR-HBF',
            CAST(DATEADD(DAY, -15, GETDATE()) AS DATE),
            CAST(DATEADD(DAY, -8, GETDATE()) AS DATE),
            'Completed',
            455.00,
            86.45,
            'EUR',
            DATEADD(DAY, -20, GETDATE()),
            DATEADD(DAY, -20, GETDATE()),
            DATEADD(DAY, -8, GETDATE())
        );

    -- Cancelled Reservations
    INSERT INTO reservations.Reservations (
        ReservationId, CustomerId, VehicleId, PickupLocationCode, DropoffLocationCode,
        PickupDate, ReturnDate, Status, TotalPriceNet, TotalPriceVat, Currency, CreatedAt, CancelledAt, CancellationReason
    )
    VALUES
        (
            NEWID(),
            @CustomerId3,
            @VehicleId3,
            'BER-HBF',
            'BER-HBF',
            CAST(DATEADD(DAY, 14, GETDATE()) AS DATE),
            CAST(DATEADD(DAY, 21, GETDATE()) AS DATE),
            'Cancelled',
            280.00,
            53.20,
            'EUR',
            DATEADD(DAY, -3, GETDATE()),
            DATEADD(DAY, -1, GETDATE()),
            'Change of plans'
        );

    PRINT 'Inserted 8 reservations';
END
ELSE
    PRINT 'Reservations already exist, skipping...';
GO

-- ============================================================================
-- Summary
-- ============================================================================

USE OrangeCarRental_Customers;
DECLARE @CustomerCount INT;
SELECT @CustomerCount = COUNT(*) FROM customers.Customers;

USE OrangeCarRental_Fleet;
DECLARE @VehicleCount INT;
SELECT @VehicleCount = COUNT(*) FROM fleet.Vehicles;

USE OrangeCarRental_Reservations;
DECLARE @ReservationCount INT;
SELECT @ReservationCount = COUNT(*) FROM reservations.Reservations;

USE OrangeCarRental_Location;
DECLARE @LocationCount INT;
SELECT @LocationCount = COUNT(*) FROM locations.Locations;

PRINT '========================================';
PRINT 'Test Data Seeding Complete';
PRINT '========================================';
PRINT 'Customers:     ' + CAST(@CustomerCount AS VARCHAR(10));
PRINT 'Vehicles:      ' + CAST(@VehicleCount AS VARCHAR(10));
PRINT 'Reservations:  ' + CAST(@ReservationCount AS VARCHAR(10));
PRINT 'Locations:     ' + CAST(@LocationCount AS VARCHAR(10));
PRINT '========================================';
GO

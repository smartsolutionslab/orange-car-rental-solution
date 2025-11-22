-- ============================================================================
-- Orange Car Rental - SQL Server Database Initialization Script
-- ============================================================================
-- This script initializes all databases for the microservices architecture
-- Designed for SQL Server 2022
-- ============================================================================

-- Create databases for each microservice
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OrangeCarRental_Customers')
BEGIN
    CREATE DATABASE OrangeCarRental_Customers;
    PRINT 'Created database: OrangeCarRental_Customers';
END
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OrangeCarRental_Fleet')
BEGIN
    CREATE DATABASE OrangeCarRental_Fleet;
    PRINT 'Created database: OrangeCarRental_Fleet';
END
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OrangeCarRental_Location')
BEGIN
    CREATE DATABASE OrangeCarRental_Location;
    PRINT 'Created database: OrangeCarRental_Location';
END
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OrangeCarRental_Reservations')
BEGIN
    CREATE DATABASE OrangeCarRental_Reservations;
    PRINT 'Created database: OrangeCarRental_Reservations';
END
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OrangeCarRental_Pricing')
BEGIN
    CREATE DATABASE OrangeCarRental_Pricing;
    PRINT 'Created database: OrangeCarRental_Pricing';
END
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OrangeCarRental_Payments')
BEGIN
    CREATE DATABASE OrangeCarRental_Payments;
    PRINT 'Created database: OrangeCarRental_Payments';
END
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OrangeCarRental_Notifications')
BEGIN
    CREATE DATABASE OrangeCarRental_Notifications;
    PRINT 'Created database: OrangeCarRental_Notifications';
END
GO

-- Summary
PRINT '========================================'
PRINT 'Orange Car Rental Databases Initialized'
PRINT '========================================'
PRINT 'Created 7 microservice databases:'
PRINT '  - OrangeCarRental_Customers'
PRINT '  - OrangeCarRental_Fleet'
PRINT '  - OrangeCarRental_Location'
PRINT '  - OrangeCarRental_Reservations'
PRINT '  - OrangeCarRental_Pricing'
PRINT '  - OrangeCarRental_Payments'
PRINT '  - OrangeCarRental_Notifications'
PRINT '========================================'
PRINT 'Next steps:'
PRINT '1. Run EF Core migrations for each service'
PRINT '2. Apply seed data if needed'
PRINT '========================================'
GO

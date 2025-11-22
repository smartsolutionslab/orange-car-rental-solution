-- ============================================================================
-- Orange Car Rental - Database Initialization Script
-- ============================================================================
-- This script initializes all databases for the microservices architecture
-- Run automatically by PostgreSQL docker container on first startup
-- ============================================================================

-- Create databases for each microservice
-- Note: These will only run on first initialization of the container
CREATE DATABASE orange_rental_customers;
CREATE DATABASE orange_rental_fleet;
CREATE DATABASE orange_rental_location;
CREATE DATABASE orange_rental_reservations;
CREATE DATABASE orange_rental_pricing;
CREATE DATABASE orange_rental_payments;
CREATE DATABASE orange_rental_notifications;

-- Grant privileges to the application user
GRANT ALL PRIVILEGES ON DATABASE orange_rental_customers TO orange_user;
GRANT ALL PRIVILEGES ON DATABASE orange_rental_fleet TO orange_user;
GRANT ALL PRIVILEGES ON DATABASE orange_rental_location TO orange_user;
GRANT ALL PRIVILEGES ON DATABASE orange_rental_reservations TO orange_user;
GRANT ALL PRIVILEGES ON DATABASE orange_rental_pricing TO orange_user;
GRANT ALL PRIVILEGES ON DATABASE orange_rental_payments TO orange_user;
GRANT ALL PRIVILEGES ON DATABASE orange_rental_notifications TO orange_user;

-- Output confirmation
DO $$
BEGIN
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Orange Car Rental Databases Initialized';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Created databases:';
    RAISE NOTICE '  - orange_rental_customers';
    RAISE NOTICE '  - orange_rental_fleet';
    RAISE NOTICE '  - orange_rental_location';
    RAISE NOTICE '  - orange_rental_reservations';
    RAISE NOTICE '  - orange_rental_pricing';
    RAISE NOTICE '  - orange_rental_payments';
    RAISE NOTICE '  - orange_rental_notifications';
    RAISE NOTICE '========================================';
END $$;

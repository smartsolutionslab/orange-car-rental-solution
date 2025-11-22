-- Test Data Seeding Script
-- Populates the database with realistic test data for development and E2E testing

-- ============================================================================
-- CUSTOMERS
-- ============================================================================

INSERT INTO customers (id, name, email, phone_number, date_of_birth, drivers_license_number, created_at, updated_at)
VALUES
    ('CUST-001', 'Max Mustermann', 'max.mustermann@example.com', '+49 30 12345678', '1985-03-15', 'D1234567890', NOW(), NOW()),
    ('CUST-002', 'Anna Schmidt', 'anna.schmidt@example.com', '+49 89 98765432', '1990-07-22', 'D0987654321', NOW(), NOW()),
    ('CUST-003', 'Thomas Müller', 'thomas.mueller@example.com', '+49 40 55555555', '1988-11-30', 'D1122334455', NOW(), NOW()),
    ('CUST-004', 'Julia Wagner', 'julia.wagner@example.com', '+49 221 66666666', '1992-05-18', 'D5544332211', NOW(), NOW()),
    ('CUST-005', 'Michael Becker', 'michael.becker@example.com', '+49 711 77777777', '1987-09-25', 'D9988776655', NOW(), NOW())
ON CONFLICT (id) DO NOTHING;

-- ============================================================================
-- VEHICLES
-- ============================================================================

INSERT INTO vehicles (id, category, model, license_plate, location, status, daily_rate, created_at, updated_at)
VALUES
    ('VEH-001', 'Kompakt', 'VW Golf', 'B-AB 1234', 'Berlin Hauptbahnhof', 'Available', 45.00, NOW(), NOW()),
    ('VEH-002', 'Kompakt', 'Ford Focus', 'M-CD 5678', 'München Flughafen', 'Available', 42.00, NOW(), NOW()),
    ('VEH-003', 'SUV', 'VW Tiguan', 'HH-EF 9012', 'Hamburg Zentrum', 'Available', 75.00, NOW(), NOW()),
    ('VEH-004', 'Mittelklasse', 'BMW 3er', 'K-GH 3456', 'Köln Messe', 'Rented', 85.00, NOW(), NOW()),
    ('VEH-005', 'Mini', 'Fiat 500', 'B-IJ 7890', 'Berlin Tegel Airport', 'Available', 35.00, NOW(), NOW()),
    ('VEH-006', 'Luxus', 'Mercedes S-Klasse', 'F-KL 1234', 'Frankfurt Hauptbahnhof', 'Available', 150.00, NOW(), NOW()),
    ('VEH-007', 'Kombi', 'VW Passat Variant', 'S-MN 5678', 'Stuttgart Hauptbahnhof', 'Available', 65.00, NOW(), NOW()),
    ('VEH-008', 'SUV', 'BMW X3', 'D-OP 9012', 'Düsseldorf Flughafen', 'Maintenance', 80.00, NOW(), NOW()),
    ('VEH-009', 'Kompakt', 'Opel Astra', 'B-QR 3456', 'Berlin Hauptbahnhof', 'Available', 40.00, NOW(), NOW()),
    ('VEH-010', 'Mittelklasse', 'Audi A4', 'M-ST 7890', 'München Flughafen', 'Available', 75.00, NOW(), NOW())
ON CONFLICT (id) DO NOTHING;

-- ============================================================================
-- RESERVATIONS
-- ============================================================================

-- Upcoming Reservations (Next week)
INSERT INTO reservations (
    id, customer_id, vehicle_id, pickup_location, return_location,
    pickup_date, return_date, status, total_price, booking_date, created_at, updated_at
)
VALUES
    (
        'RES-2001',
        'CUST-001',
        'VEH-001',
        'Berlin Hauptbahnhof',
        'Berlin Hauptbahnhof',
        NOW() + INTERVAL '3 days',
        NOW() + INTERVAL '10 days',
        'Confirmed',
        315.00,
        NOW() - INTERVAL '2 days',
        NOW(),
        NOW()
    ),
    (
        'RES-2002',
        'CUST-002',
        'VEH-003',
        'Hamburg Zentrum',
        'Hamburg Zentrum',
        NOW() + INTERVAL '5 days',
        NOW() + INTERVAL '8 days',
        'Confirmed',
        225.00,
        NOW() - INTERVAL '1 day',
        NOW(),
        NOW()
    )
ON CONFLICT (id) DO NOTHING;

-- Active Reservations (Currently rented)
INSERT INTO reservations (
    id, customer_id, vehicle_id, pickup_location, return_location,
    pickup_date, return_date, status, total_price, booking_date, created_at, updated_at
)
VALUES
    (
        'RES-1001',
        'CUST-003',
        'VEH-004',
        'Köln Messe',
        'Köln Messe',
        NOW() - INTERVAL '2 days',
        NOW() + INTERVAL '5 days',
        'Active',
        595.00,
        NOW() - INTERVAL '5 days',
        NOW(),
        NOW()
    )
ON CONFLICT (id) DO NOTHING;

-- Pending Reservations (Awaiting payment)
INSERT INTO reservations (
    id, customer_id, vehicle_id, pickup_location, return_location,
    pickup_date, return_date, status, total_price, booking_date, created_at, updated_at
)
VALUES
    (
        'RES-3001',
        'CUST-004',
        'VEH-006',
        'Frankfurt Hauptbahnhof',
        'München Flughafen',
        NOW() + INTERVAL '7 days',
        NOW() + INTERVAL '10 days',
        'Pending',
        450.00,
        NOW(),
        NOW(),
        NOW()
    )
ON CONFLICT (id) DO NOTHING;

-- Completed Reservations (Past rentals)
INSERT INTO reservations (
    id, customer_id, vehicle_id, pickup_location, return_location,
    pickup_date, return_date, status, total_price, booking_date, created_at, updated_at
)
VALUES
    (
        'RES-0001',
        'CUST-001',
        'VEH-002',
        'München Flughafen',
        'München Flughafen',
        NOW() - INTERVAL '30 days',
        NOW() - INTERVAL '23 days',
        'Completed',
        294.00,
        NOW() - INTERVAL '35 days',
        NOW(),
        NOW()
    ),
    (
        'RES-0002',
        'CUST-002',
        'VEH-005',
        'Berlin Tegel Airport',
        'Berlin Hauptbahnhof',
        NOW() - INTERVAL '20 days',
        NOW() - INTERVAL '15 days',
        'Completed',
        175.00,
        NOW() - INTERVAL '25 days',
        NOW(),
        NOW()
    ),
    (
        'RES-0003',
        'CUST-005',
        'VEH-007',
        'Stuttgart Hauptbahnhof',
        'Stuttgart Hauptbahnhof',
        NOW() - INTERVAL '15 days',
        NOW() - INTERVAL '8 days',
        'Completed',
        455.00,
        NOW() - INTERVAL '20 days',
        NOW(),
        NOW()
    )
ON CONFLICT (id) DO NOTHING;

-- Cancelled Reservations
INSERT INTO reservations (
    id, customer_id, vehicle_id, pickup_location, return_location,
    pickup_date, return_date, status, total_price, booking_date, cancellation_date, cancellation_reason, created_at, updated_at
)
VALUES
    (
        'RES-9001',
        'CUST-003',
        'VEH-009',
        'Berlin Hauptbahnhof',
        'Berlin Hauptbahnhof',
        NOW() + INTERVAL '14 days',
        NOW() + INTERVAL '21 days',
        'Cancelled',
        280.00,
        NOW() - INTERVAL '3 days',
        NOW() - INTERVAL '1 day',
        'Change of plans',
        NOW(),
        NOW()
    )
ON CONFLICT (id) DO NOTHING;

-- ============================================================================
-- LOCATIONS
-- ============================================================================

INSERT INTO locations (id, name, address, city, postal_code, country, phone_number, email, opening_hours, created_at, updated_at)
VALUES
    ('LOC-001', 'Berlin Hauptbahnhof', 'Europaplatz 1', 'Berlin', '10557', 'Deutschland', '+49 30 11111111', 'berlin.hbf@orangecarrental.com', 'Mo-Su 06:00-22:00', NOW(), NOW()),
    ('LOC-002', 'München Flughafen', 'Nordallee 25', 'München', '85356', 'Deutschland', '+49 89 22222222', 'muenchen.airport@orangecarrental.com', 'Mo-Su 05:00-23:00', NOW(), NOW()),
    ('LOC-003', 'Hamburg Zentrum', 'Mönckebergstraße 1', 'Hamburg', '20095', 'Deutschland', '+49 40 33333333', 'hamburg.zentrum@orangecarrental.com', 'Mo-Sa 08:00-20:00', NOW(), NOW()),
    ('LOC-004', 'Köln Messe', 'Messeplatz 1', 'Köln', '50679', 'Deutschland', '+49 221 44444444', 'koeln.messe@orangecarrental.com', 'Mo-Su 07:00-21:00', NOW(), NOW()),
    ('LOC-005', 'Frankfurt Hauptbahnhof', 'Am Hauptbahnhof', 'Frankfurt', '60329', 'Deutschland', '+49 69 55555555', 'frankfurt.hbf@orangecarrental.com', 'Mo-Su 06:00-22:00', NOW(), NOW())
ON CONFLICT (id) DO NOTHING;

-- ============================================================================
-- Summary
-- ============================================================================

DO $$
DECLARE
    customer_count INTEGER;
    vehicle_count INTEGER;
    reservation_count INTEGER;
    location_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO customer_count FROM customers;
    SELECT COUNT(*) INTO vehicle_count FROM vehicles;
    SELECT COUNT(*) INTO reservation_count FROM reservations;
    SELECT COUNT(*) INTO location_count FROM locations;

    RAISE NOTICE '========================================';
    RAISE NOTICE 'Test Data Seeding Complete';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Customers:     %', customer_count;
    RAISE NOTICE 'Vehicles:      %', vehicle_count;
    RAISE NOTICE 'Reservations:  %', reservation_count;
    RAISE NOTICE 'Locations:     %', location_count;
    RAISE NOTICE '========================================';
END $$;

/**
 * Test data fixtures for Call Center Portal E2E tests
 */

export const testReservations = {
  /**
   * Sample reservation data for testing
   */
  confirmed: {
    id: '123e4567-e89b-12d3-a456-426614174000',
    customerId: '11111111-1111-1111-1111-111111111111',
    vehicleId: 'veh-001',
    status: 'Confirmed',
    pickupDate: '2025-12-01',
    returnDate: '2025-12-05',
    pickupLocationCode: 'MUC',
    dropoffLocationCode: 'MUC',
    totalPriceGross: 400.00
  },
  pending: {
    id: '223e4567-e89b-12d3-a456-426614174001',
    customerId: '22222222-2222-2222-2222-222222222222',
    vehicleId: 'veh-002',
    status: 'Pending',
    pickupDate: '2025-12-10',
    returnDate: '2025-12-15',
    pickupLocationCode: 'BER',
    dropoffLocationCode: 'BER',
    totalPriceGross: 300.00
  }
};

export const testCustomers = {
  /**
   * Test customer for search and profile tests
   */
  existing: {
    id: '11111111-1111-1111-1111-111111111111',
    firstName: 'Max',
    lastName: 'Mustermann',
    email: 'max.mustermann@example.com',
    phoneNumber: '+49 30 12345678',
    dateOfBirth: '1990-01-15',
    address: {
      street: 'Musterstraße 123',
      city: 'Berlin',
      postalCode: '10115',
      country: 'Deutschland'
    },
    driversLicense: {
      licenseNumber: 'B1234567890',
      licenseIssueCountry: 'Deutschland',
      licenseIssueDate: '2015-06-01',
      licenseExpiryDate: '2030-06-01'
    }
  },
  searchable: {
    email: 'test@orange-rental.de',
    lastName: 'Schmidt',
    phoneNumber: '+49 30 98765432'
  }
};

export const testVehicles = {
  available: {
    id: '123e4567-e89b-12d3-a456-426614174000',
    name: 'VW Golf',
    categoryCode: 'MITTEL',
    locationCode: 'MUC',
    status: 'Available',
    dailyRate: 59.50
  },
  rented: {
    id: '987e6543-e89b-12d3-a456-426614174001',
    name: 'BMW 3er',
    categoryCode: 'PREMIUM',
    locationCode: 'BER',
    status: 'Rented',
    dailyRate: 89.90
  }
};

export const testLocations = {
  munich: {
    code: 'MUC',
    name: 'München Hauptbahnhof',
    city: 'München',
    address: 'Bayerstraße 10A',
    postalCode: '80335'
  },
  berlin: {
    code: 'BER',
    name: 'Berlin Hauptbahnhof',
    city: 'Berlin',
    address: 'Europaplatz 1',
    postalCode: '10557'
  }
};

export const testDates = {
  /**
   * Get a date N days from now in YYYY-MM-DD format
   */
  futureDate: (daysFromNow: number): string => {
    const date = new Date();
    date.setDate(date.getDate() + daysFromNow);
    return date.toISOString().split('T')[0]!;
  },
  /**
   * Get a date N days ago in YYYY-MM-DD format
   */
  pastDate: (daysAgo: number): string => {
    const date = new Date();
    date.setDate(date.getDate() - daysAgo);
    return date.toISOString().split('T')[0]!;
  }
};

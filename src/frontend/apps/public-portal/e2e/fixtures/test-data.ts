/**
 * Test data fixtures for E2E tests
 */

export const testUsers = {
  /**
   * Existing registered user for login tests
   */
  registered: {
    email: 'test.user@orange-rental.de',
    password: 'TestPassword123!',
    firstName: 'Max',
    lastName: 'Mustermann',
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

  /**
   * New user data for registration tests
   */
  newUser: {
    email: `test.${Date.now()}@orange-rental.de`,
    password: 'NewPassword123!',
    confirmPassword: 'NewPassword123!',
    firstName: 'Anna',
    lastName: 'Schmidt',
    phoneNumber: '+49 30 98765432',
    dateOfBirth: '1995-03-20'
  },

  /**
   * Guest user for booking without authentication
   */
  guest: {
    firstName: 'Hans',
    lastName: 'Müller',
    email: 'hans.mueller@example.com',
    phoneNumber: '+49 30 55555555',
    dateOfBirth: '1988-07-10',
    address: {
      street: 'Teststraße 456',
      city: 'München',
      postalCode: '80331',
      country: 'Deutschland'
    },
    driversLicense: {
      licenseNumber: 'M9876543210',
      licenseIssueCountry: 'Deutschland',
      licenseIssueDate: '2012-03-15',
      licenseExpiryDate: '2027-03-15'
    }
  }
};

export const testVehicles = {
  available: {
    id: '123e4567-e89b-12d3-a456-426614174000',
    name: 'VW Golf',
    categoryCode: 'MITTEL',
    dailyRate: 59.50
  },
  unavailable: {
    id: '987e6543-e89b-12d3-a456-426614174001',
    name: 'BMW 3er',
    categoryCode: 'PREMIUM',
    dailyRate: 89.90
  }
};

export const testBooking = {
  pickupDate: () => {
    const date = new Date();
    date.setDate(date.getDate() + 7); // 7 days from now
    return date.toISOString().split('T')[0];
  },
  returnDate: () => {
    const date = new Date();
    date.setDate(date.getDate() + 14); // 14 days from now
    return date.toISOString().split('T')[0];
  },
  locationCode: 'BER-HBF'
};

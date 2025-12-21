/**
 * Mock customer data for testing
 */
import type { CustomerId } from '@orange-car-rental/reservation-api';
import type {
  EmailAddress,
  PhoneNumber,
  FirstName,
  LastName,
  PostalCode,
  CountryCode,
  ISODateString,
} from '../types';
import { TEST_CUSTOMER_IDS } from './mock-reservations';

/**
 * Customer profile structure for testing
 */
export interface MockCustomerProfile {
  id: CustomerId;
  email: EmailAddress;
  firstName: FirstName;
  lastName: LastName;
  phoneNumber: PhoneNumber;
  dateOfBirth: ISODateString;
  address?: {
    street: string;
    city: string;
    postalCode: PostalCode;
    country: CountryCode;
  };
  driversLicense?: {
    number: string;
    issuingCountry: CountryCode;
    expiryDate: ISODateString;
  };
}

/**
 * Create a mock customer profile with sensible defaults
 */
export function createMockCustomer(overrides: Partial<MockCustomerProfile> = {}): MockCustomerProfile {
  return {
    id: TEST_CUSTOMER_IDS.HANS_MUELLER,
    email: 'hans.mueller@example.com' as EmailAddress,
    firstName: 'Hans' as FirstName,
    lastName: 'Müller' as LastName,
    phoneNumber: '+49 170 1234567' as PhoneNumber,
    dateOfBirth: '1985-03-15' as ISODateString,
    address: {
      street: 'Musterstraße 123',
      city: 'Berlin',
      postalCode: '10115' as PostalCode,
      country: 'DE' as CountryCode,
    },
    driversLicense: {
      number: 'B123456789',
      issuingCountry: 'DE' as CountryCode,
      expiryDate: getFutureLicenseDate(),
    },
    ...overrides,
  };
}

function getFutureLicenseDate(): ISODateString {
  const date = new Date();
  date.setFullYear(date.getFullYear() + 5);
  return date.toISOString().split('T')[0]! as ISODateString;
}

/**
 * Pre-built mock customers for common test scenarios
 */
export const MOCK_CUSTOMERS = {
  /** Hans Müller - Standard German customer */
  HANS_MUELLER: createMockCustomer({
    id: TEST_CUSTOMER_IDS.HANS_MUELLER,
    email: 'hans.mueller@example.com' as EmailAddress,
    firstName: 'Hans' as FirstName,
    lastName: 'Müller' as LastName,
    phoneNumber: '+49 170 1234567' as PhoneNumber,
    dateOfBirth: '1985-03-15' as ISODateString,
    address: {
      street: 'Musterstraße 123',
      city: 'Berlin',
      postalCode: '10115' as PostalCode,
      country: 'DE' as CountryCode,
    },
  }),

  /** Anna Schmidt - Female customer */
  ANNA_SCHMIDT: createMockCustomer({
    id: TEST_CUSTOMER_IDS.ANNA_SCHMIDT,
    email: 'anna.schmidt@example.com' as EmailAddress,
    firstName: 'Anna' as FirstName,
    lastName: 'Schmidt' as LastName,
    phoneNumber: '+49 171 9876543' as PhoneNumber,
    dateOfBirth: '1990-07-22' as ISODateString,
    address: {
      street: 'Hauptstraße 45',
      city: 'München',
      postalCode: '80331' as PostalCode,
      country: 'DE' as CountryCode,
    },
  }),

  /** Max Weber - Customer from Frankfurt */
  MAX_WEBER: createMockCustomer({
    id: TEST_CUSTOMER_IDS.MAX_WEBER,
    email: 'max.weber@example.com' as EmailAddress,
    firstName: 'Max' as FirstName,
    lastName: 'Weber' as LastName,
    phoneNumber: '+49 172 5551234' as PhoneNumber,
    dateOfBirth: '1978-11-08' as ISODateString,
    address: {
      street: 'Goethestraße 78',
      city: 'Frankfurt',
      postalCode: '60313' as PostalCode,
      country: 'DE' as CountryCode,
    },
  }),
} as const;

/**
 * User profile for authentication testing
 */
export interface MockUserProfile {
  id: string;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
}

/**
 * Create a mock user profile for auth testing
 */
export function createMockUserProfile(overrides: Partial<MockUserProfile> = {}): MockUserProfile {
  return {
    id: TEST_CUSTOMER_IDS.HANS_MUELLER as string,
    username: 'hans.mueller',
    email: 'hans.mueller@example.com',
    firstName: 'Hans',
    lastName: 'Müller',
    ...overrides,
  };
}

/**
 * Get an array of mock customers for list testing
 */
export function getMockCustomerList(count: number = 3): MockCustomerProfile[] {
  const allCustomers = Object.values(MOCK_CUSTOMERS);
  return allCustomers.slice(0, Math.min(count, allCustomers.length));
}

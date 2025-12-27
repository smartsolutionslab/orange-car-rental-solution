/**
 * Mock authentication data for testing
 */
import type { EmailAddress, PhoneNumber, FirstName, LastName } from '../types';

/**
 * Test email addresses
 */
export const TEST_EMAILS = {
  VALID: 'test@example.com' as EmailAddress,
  GUEST: 'guest@example.com' as EmailAddress,
  ADMIN: 'admin@example.de' as EmailAddress,
  INVALID: 'invalid-email' as unknown as EmailAddress,
} as const;

/**
 * Test passwords
 */
export const TEST_PASSWORDS = {
  VALID: 'password123',
  STRONG: 'StrongP@ssw0rd!',
  WEAK: 'weakpassword',
  SHORT: 'short',
  MISMATCH: 'DifferentP@ssw0rd!',
} as const;

/**
 * Test personal information
 */
export const TEST_PERSONAL_INFO = {
  FIRST_NAME: 'Max' as FirstName,
  LAST_NAME: 'Mustermann' as LastName,
  PHONE: '+49 123 456789' as PhoneNumber,
  PHONE_INVALID: 'invalid' as PhoneNumber,
  DATE_OF_BIRTH_VALID: getValidDateOfBirth(),
  DATE_OF_BIRTH_UNDERAGE: getUnderageDateOfBirth(),
} as const;

/**
 * Test user profiles for authenticated scenarios
 */
export const TEST_USER_PROFILES = {
  STANDARD: {
    id: '11111111-1111-1111-1111-111111111111',
    username: 'testuser',
    email: 'test@example.com',
    firstName: 'Test',
    lastName: 'User',
  },
  ADMIN: {
    id: '22222222-2222-2222-2222-222222222222',
    username: 'admin',
    email: 'admin@example.de',
    firstName: 'Admin',
    lastName: 'User',
  },
} as const;

/**
 * Get a valid date of birth (25 years ago)
 */
function getValidDateOfBirth(): string {
  const today = new Date();
  const validDate = new Date(today.getFullYear() - 25, today.getMonth(), today.getDate());
  return validDate.toISOString().split('T')[0]!;
}

/**
 * Get an underage date of birth (17 years ago)
 */
function getUnderageDateOfBirth(): string {
  const today = new Date();
  const underageDate = new Date(today.getFullYear() - 17, today.getMonth(), today.getDate());
  return underageDate.toISOString().split('T')[0]!;
}

/**
 * Create a valid registration form data object
 */
export function createValidRegistrationData() {
  return {
    email: TEST_EMAILS.VALID,
    password: TEST_PASSWORDS.STRONG,
    confirmPassword: TEST_PASSWORDS.STRONG,
    firstName: TEST_PERSONAL_INFO.FIRST_NAME,
    lastName: TEST_PERSONAL_INFO.LAST_NAME,
    phoneNumber: TEST_PERSONAL_INFO.PHONE,
    dateOfBirth: TEST_PERSONAL_INFO.DATE_OF_BIRTH_VALID,
    acceptTerms: true,
    acceptPrivacy: true,
    acceptMarketing: false,
  };
}

/**
 * Create a valid login form data object
 */
export function createValidLoginData() {
  return {
    email: TEST_EMAILS.VALID,
    password: TEST_PASSWORDS.VALID,
    rememberMe: true,
  };
}

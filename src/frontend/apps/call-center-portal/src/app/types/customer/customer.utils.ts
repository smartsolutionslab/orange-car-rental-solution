/**
 * Customer utility functions
 */
import type { CustomerDisplayName } from '@orange-car-rental/customer-api';
import {
  isLicenseExpired as checkLicenseExpired,
  isLicenseExpiringSoon,
} from '@orange-car-rental/ui-components';
import type { Customer } from './customer.type';

/**
 * Helper function to create display name
 */
export const getCustomerDisplayName = (
  customer: Pick<Customer, 'firstName' | 'lastName'>,
): CustomerDisplayName => `${customer.firstName} ${customer.lastName}`;

/**
 * Helper function to check if license is expired
 */
export const isLicenseExpired = (customer: Pick<Customer, 'driversLicense'>): boolean => {
  if (!customer.driversLicense?.expiryDate) return false;
  return checkLicenseExpired(customer.driversLicense.expiryDate);
};

/**
 * Helper function to check if license expires soon (within days)
 */
export const licenseExpiresSoon = (
  customer: Pick<Customer, 'driversLicense'>,
  days = 30,
): boolean => {
  if (!customer.driversLicense?.expiryDate) return false;
  return isLicenseExpiringSoon(customer.driversLicense.expiryDate, days);
};

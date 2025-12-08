/**
 * Customer utility functions
 */
import type { CustomerDisplayName } from '@orange-car-rental/customer-api';
import type { Customer } from './customer.type';

/**
 * Helper function to create display name
 */
export const getCustomerDisplayName = (
  customer: Pick<Customer, 'firstName' | 'lastName'>
): CustomerDisplayName => `${customer.firstName} ${customer.lastName}`;

/**
 * Helper function to check if license is expired
 */
export const isLicenseExpired = (customer: Pick<Customer, 'licenseExpiryDate'>): boolean => {
  const expiryDate = new Date(customer.licenseExpiryDate);
  return expiryDate < new Date();
};

/**
 * Helper function to check if license expires soon (within days)
 */
export const licenseExpiresSoon = (
  customer: Pick<Customer, 'licenseExpiryDate'>,
  days = 30
): boolean => {
  const expiryDate = new Date(customer.licenseExpiryDate);
  const warningDate = new Date();
  warningDate.setDate(warningDate.getDate() + days);
  return expiryDate <= warningDate && expiryDate >= new Date();
};

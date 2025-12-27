/**
 * Helper function to create display name
 */
import type { CustomerProfile } from './customer-profile.dto';
import type { CustomerDisplayName } from './customer-display-name.type';

export const getCustomerDisplayName = (
  customer: Pick<CustomerProfile, 'firstName' | 'lastName'>
): CustomerDisplayName => `${customer.firstName} ${customer.lastName}`;

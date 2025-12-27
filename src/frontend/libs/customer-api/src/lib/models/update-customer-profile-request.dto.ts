/**
 * Customer profile update request
 * Uses Omit to exclude read-only fields from CustomerProfile
 */
import type { CustomerProfile } from './customer-profile.dto';

export type UpdateCustomerProfileRequest = Omit<CustomerProfile, 'id' | 'createdAt' | 'updatedAt'>;

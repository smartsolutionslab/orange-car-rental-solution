import {
  CustomerId,
  EmailAddress,
  PhoneNumber,
  ISODateString,
  AddressDetails,
  DriversLicenseDetails
} from './reservation.model';

/**
 * Complete customer profile
 * Used for getting and updating customer information
 */
export interface CustomerProfile {
  readonly id: CustomerId;
  readonly firstName: string;
  readonly lastName: string;
  readonly email: EmailAddress;
  readonly phoneNumber: PhoneNumber;
  readonly dateOfBirth: ISODateString;
  readonly address: AddressDetails;
  readonly driversLicense?: DriversLicenseDetails;
  readonly createdAt?: ISODateString;
  readonly updatedAt?: ISODateString;
}

/**
 * Customer profile update request
 * Uses Omit to exclude read-only fields from CustomerProfile
 */
export type UpdateCustomerProfileRequest = Omit<CustomerProfile, 'id' | 'createdAt' | 'updatedAt'>;

/**
 * Partial customer update - allows updating individual fields
 */
export type PartialCustomerUpdate = Partial<UpdateCustomerProfileRequest>;

/**
 * Customer display name helper type
 */
export type CustomerDisplayName = `${string} ${string}`;

/**
 * Helper function to create display name
 */
export const getCustomerDisplayName = (customer: Pick<CustomerProfile, 'firstName' | 'lastName'>): CustomerDisplayName =>
  `${customer.firstName} ${customer.lastName}`;

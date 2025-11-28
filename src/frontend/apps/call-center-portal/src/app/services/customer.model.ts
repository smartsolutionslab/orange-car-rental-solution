import {
  CustomerId,
  EmailAddress,
  PhoneNumber,
  ISODateString,
  LicenseNumber,
  CountryCode,
  PostalCode
} from './reservation.model';
import { CityName } from './vehicle.model';

/**
 * Customer details
 */
export interface Customer {
  readonly id: CustomerId;
  readonly firstName: string;
  readonly lastName: string;
  readonly email: EmailAddress;
  readonly phoneNumber: PhoneNumber;
  readonly dateOfBirth: ISODateString;
  readonly street: string;
  readonly city: CityName;
  readonly postalCode: PostalCode;
  readonly country: CountryCode;
  readonly licenseNumber: LicenseNumber;
  readonly licenseIssueCountry: CountryCode;
  readonly licenseIssueDate: ISODateString;
  readonly licenseExpiryDate: ISODateString;
  readonly createdAt: ISODateString;
  readonly updatedAt?: ISODateString;
}

/**
 * Customer search query
 */
export interface CustomerSearchQuery {
  readonly email?: EmailAddress;
  readonly phoneNumber?: PhoneNumber;
  readonly lastName?: string;
  readonly pageNumber?: number;
  readonly pageSize?: number;
}

/**
 * Customer search result
 */
export interface CustomerSearchResult {
  readonly customers: Customer[];
  readonly totalCount: number;
  readonly pageNumber: number;
  readonly pageSize: number;
  readonly totalPages: number;
  readonly hasPreviousPage?: boolean;
  readonly hasNextPage?: boolean;
}

/**
 * Update customer request
 * Uses Omit to create the request type from Customer
 */
export type UpdateCustomerRequest = Omit<Customer, 'id' | 'createdAt' | 'updatedAt'>;

/**
 * Partial customer update
 */
export type PartialCustomerUpdate = Partial<UpdateCustomerRequest>;

/**
 * Customer display name helper
 */
export type CustomerDisplayName = `${string} ${string}`;

/**
 * Helper function to create display name
 */
export const getCustomerDisplayName = (customer: Pick<Customer, 'firstName' | 'lastName'>): CustomerDisplayName =>
  `${customer.firstName} ${customer.lastName}`;

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
export const licenseExpiresSoon = (customer: Pick<Customer, 'licenseExpiryDate'>, days = 30): boolean => {
  const expiryDate = new Date(customer.licenseExpiryDate);
  const warningDate = new Date();
  warningDate.setDate(warningDate.getDate() + days);
  return expiryDate <= warningDate && expiryDate >= new Date();
};

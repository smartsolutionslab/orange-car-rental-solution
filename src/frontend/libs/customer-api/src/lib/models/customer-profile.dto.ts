/**
 * Complete customer profile
 * Used for getting and updating customer information
 */
import type { ISODateString, EmailAddress, PhoneNumber } from '@orange-car-rental/shared';
import type { CustomerId, AddressDetails, DriversLicenseDetails } from '@orange-car-rental/reservation-api';

export type CustomerProfile = {
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
};

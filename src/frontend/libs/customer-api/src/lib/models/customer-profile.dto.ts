/**
 * Complete customer profile
 * Used for getting and updating customer information
 */
import type {
  ISODateString,
  EmailAddress,
  PhoneNumber,
  FirstName,
  LastName,
} from '@orange-car-rental/shared';
import type { CustomerId, AddressDetails, DriversLicenseDetails } from '@orange-car-rental/reservation-api';

export type CustomerProfile = {
  readonly id: CustomerId;
  readonly firstName: FirstName;
  readonly lastName: LastName;
  readonly email: EmailAddress;
  readonly phoneNumber: PhoneNumber;
  readonly dateOfBirth: ISODateString;
  readonly address: AddressDetails;
  readonly driversLicense?: DriversLicenseDetails;
  readonly createdAt?: ISODateString;
  readonly updatedAt?: ISODateString;
};

/**
 * Customer details matching backend CustomerDto structure
 */
import type { CustomerId } from '@orange-car-rental/customer-api';
import type {
  EmailAddress,
  PhoneNumber,
  ISODateString,
  PostalCode,
  CountryCode,
  FirstName,
  LastName,
} from '@orange-car-rental/shared';
import type { LicenseNumber } from '@orange-car-rental/reservation-api';
import type { CityName, StreetAddress } from '@orange-car-rental/location-api';

/**
 * Address details nested object
 */
export interface CustomerAddress {
  readonly street: StreetAddress;
  readonly city: CityName;
  readonly postalCode: PostalCode;
  readonly country: CountryCode;
}

/**
 * Driver's license details nested object
 */
export interface CustomerDriversLicense {
  readonly licenseNumber: LicenseNumber;
  readonly issueCountry: CountryCode;
  readonly issueDate: ISODateString;
  readonly expiryDate: ISODateString;
  readonly isValid?: boolean;
  readonly isEuLicense?: boolean;
  readonly daysUntilExpiry?: number;
}

/**
 * Customer details matching backend CustomerDto
 */
export interface Customer {
  readonly id: CustomerId;
  readonly firstName: FirstName;
  readonly lastName: LastName;
  readonly fullName?: string;
  readonly email: EmailAddress;
  readonly phoneNumber: PhoneNumber;
  readonly phoneNumberFormatted?: string;
  readonly dateOfBirth: ISODateString;
  readonly age?: number;
  readonly address: CustomerAddress | null;
  readonly driversLicense: CustomerDriversLicense | null;
  readonly status?: string;
  readonly canMakeReservation?: boolean;
  readonly registeredAtUtc?: ISODateString;
  readonly updatedAtUtc?: ISODateString;
}
